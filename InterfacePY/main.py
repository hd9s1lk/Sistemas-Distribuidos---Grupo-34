from flask import Flask, render_template_string
import pyodbc

app = Flask(__name__)

def ler_dados():
    # Tenta autenticação Windows (funciona em qualquer PC com permissões na BD)
    conn = pyodbc.connect(
    'DRIVER={ODBC Driver 17 for SQL Server};'
    'SERVER=(localdb)\\MSSQLLocalDB;'
    'DATABASE=SD;'
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

if __name__ == '__main__':
    app.run(debug=True, port=5067)
