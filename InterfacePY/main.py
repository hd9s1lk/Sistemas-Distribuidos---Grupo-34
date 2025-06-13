from flask import Flask, render_template_string, request, jsonify
import pyodbc

app = Flask(__name__)

def ler_dados():
    conn = pyodbc.connect(
        'DRIVER={ODBC Driver 17 for SQL Server};'
        'SERVER=(localdb)\\MSSQLLocalDB;'
        'DATABASE=sd;'
        'Trusted_Connection=yes;'
    )
    cursor = conn.cursor()
    cursor.execute("SELECT ID, IPWavy, WavyProcessamento, Temperatura, Velocidade_Ondas, Altura_Ondas, Profundidade, Data_dados, Estado FROM dados_agregador")
    resultados = cursor.fetchall()
    dados = [list(linha) for linha in resultados]
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
    id_val = data.get('id')
    if not id_val:
        return jsonify({'success': False, 'error': 'ID não fornecido'}), 400
    try:
        conn = pyodbc.connect(
            'DRIVER={ODBC Driver 17 for SQL Server};'
            'SERVER=(localdb)\\MSSQLLocalDB;'
            'DATABASE=sd;'
            'Trusted_Connection=yes;'
        )
        cursor = conn.cursor()
        cursor.execute("DELETE FROM dados_agregador WHERE ID = ?", id_val)
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
            INSERT INTO dados_agregador 
            (IPWavy, WavyProcessamento, Temperatura, Velocidade_Ondas, Altura_Ondas, Profundidade, Data_dados, Estado)
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
