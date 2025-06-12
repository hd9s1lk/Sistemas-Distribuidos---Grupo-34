from flask import Flask, render_template_string, request, jsonify
import pyodbc

app = Flask(__name__)

def ler_dados():
    # Tenta autenticação Windows (funciona em qualquer PC com permissões na BD)
    conn = pyodbc.connect(
    'DRIVER={ODBC Driver 17 for SQL Server};'
    'SERVER=(localdb)\\MSSQLLocalDB;'
    'DATABASE=sd;'
    'Trusted_Connection=yes;'
)
    cursor = conn.cursor()
    cursor.execute("SELECT IPWavy, WavyProcessamento, Temperatura, Velocidade_Ondas, Altura_Ondas, Profundidade, Data_dados, Estado FROM dados_agregador")
    resultados = cursor.fetchall()
    dados = []
    for idx, linha in enumerate(resultados):
        linha = list(linha)
        linha.insert(0, str(idx+1))  # ID na primeira coluna
        dados.append(linha)
    cursor.close()
    conn.close()
    return dados

colunas = [
    'ID', 'IPWavy', 'WavyProcessamento', 'Temperatura', 'Velocidade_Ondas',
    'Altura_Ondas', 'Profundidade', 'Data_dados', 'Estado'
]

@app.route('/')
def tabela():
    dados = ler_dados()
    with open('tabela.html', 'r', encoding='utf-8') as f:
        html = f.read()
    return render_template_string(html, colunas=colunas, dados=dados)

@app.route('/eliminar_wavy', methods=['POST'])
def eliminar_wavy():
    data = request.get_json()
    id_str = data.get('id')
    ipwavy = data.get('ipwavy')
    if not id_str or not ipwavy:
        return jsonify({'success': False, 'error': 'Dados insuficientes'}), 400
    try:
        conn = pyodbc.connect(
            'DRIVER={ODBC Driver 17 for SQL Server};'
            'SERVER=(localdb)\\MSSQLLocalDB;'
            'DATABASE=sd;'
            'Trusted_Connection=yes;'
        )
        cursor = conn.cursor()
        # Busca o IPWavy correspondente ao ID (ID é índice da lista, não da BD)
        cursor.execute("SELECT IPWavy FROM dados_agregador")
        resultados = cursor.fetchall()
        idx = int(id_str) - 1
        if idx < 0 or idx >= len(resultados):
            cursor.close()
            conn.close()
            return jsonify({'success': False, 'error': 'ID inválido'}), 400
        ipwavy_db = resultados[idx][0]
        if ipwavy_db != ipwavy:
            cursor.close()
            conn.close()
            return jsonify({'success': False, 'error': 'IPWavy não corresponde'}), 400
        cursor.execute("DELETE FROM dados_agregador WHERE IPWavy = ?", ipwavy)
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/adicionar_wavy', methods=['POST'])
def adicionar_wavy():
    data = request.get_json()
    try:
        conn = pyodbc.connect(
            'DRIVER={ODBC Driver 17 for SQL Server};'
            'SERVER=(localdb)\\MSSQLLocalDB;'
            'DATABASE=sd;'
            'Trusted_Connection=yes;'
        )
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO dados_agregador (IPWavy, WavyProcessamento, Temperatura, Velocidade_Ondas, Altura_Ondas, Profundidade, Data_dados, Estado)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """, (
            data['IPWavy'],
            data['WavyProcessamento'],
            data['Temperatura'],
            data['Velocidade_Ondas'],
            data['Altura_Ondas'],
            data['Profundidade'],
            data['Data_dados'],
            data['Estado']
        ))
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'success': False, 'error': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, port=5067)
