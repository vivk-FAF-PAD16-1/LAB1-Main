import json

import redis
from flask import Blueprint, request, jsonify

from config import RedisConfig

cache = Blueprint("crud", __name__, url_prefix="/cache")
db = redis.Redis(host=RedisConfig.host, port=RedisConfig.port, db=0)


@cache.route("get/<string:key>")
def get_value(key):
    token = request.headers.get("Authorization")

    if not token:
        return jsonify({"msg": "Authorization token is not found"}), 400

    value = db.hget(token, key)
    if not value:
        return jsonify(None)
    return jsonify(json.loads(value))


@cache.route("post", methods=["POST"])
def post_value():
    token = request.headers.get("Authorization")

    if not token:
        return jsonify({"msg": "Authorization token is not found"}), 401

    post_body = request.get_json()
    for key, value in post_body.items():
        try:
            db.hset(token, key, json.dumps(value))
        except redis.exceptions.DataError:
            return jsonify({"msg": "invalid input type"}), 400

    return jsonify({"msg": "data sent successfully"}), 200


@cache.route("delete/<string:key>", methods=["DELETE"])
def delete_value(key):
    token = request.headers.get("Authorization")

    if not token:
        return jsonify({"msg": "Authorization token is not found"}), 400
    try:
        db.hdel(token, key)
    except redis.exceptions.DataError:
        return jsonify({"msg": "invalid input type"})
    return jsonify({"msg": "data deleted successfully"})
