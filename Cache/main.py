import redis
from flask import Flask, request
from config import RedisConfig


def create_app():
    app = Flask(__name__)
    app.debug = True

    from view import cache
    app.register_blueprint(cache)

    return app
