from const import SERVER_HOST, SERVER_PORT
from main import create_app

if __name__ == "__main__":
    app = create_app()
    app.run(host=SERVER_HOST, port=SERVER_PORT)
