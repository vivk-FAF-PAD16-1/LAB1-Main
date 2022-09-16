import redis
from flask import Blueprint, request, jsonify

from config import RedisConfig

cache = Blueprint("crud", __name__, url_prefix="/cache")
db = redis.Redis(host=RedisConfig.host, port=RedisConfig.port, db=0)


@cache.route("get/<key>")
def get_value(key):
    token = request.headers.get("Authorization")

    if not token:
        return jsonify({"msg": "Authorization token is not found"}), 400

    value = db.hget(token, key)

    return jsonify({"msg": value})


@cache.route("post", methods=["POST"])
def post_value():
    token = request.headers.get("Authorization")

    if not token:
        return jsonify({"msg": "Authorization token is not found"}), 401

    post_body = request.get_json()

    if not post_body.get("data"):
        return jsonify({"msg": "[data] field was not found"}), 400

    for key, value in post_body["data"].items():
        try:
            db.hset(token, key, value)
        except redis.exceptions.DataError:
            return jsonify({"msg": "invalid input type"}), 400

    return jsonify({"msg": "data sent successfully"}), 200
