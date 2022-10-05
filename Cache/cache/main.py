from flask import Flask


def create_app():
    app = Flask(__name__)
    app.debug = True

    from view import cache
    app.register_blueprint(cache)

    return app
