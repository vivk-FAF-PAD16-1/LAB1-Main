import redis
from config import RedisConfig

redis = redis.Redis(host=RedisConfig.host, port=RedisConfig.port, db=0)

redis.set("first", '1', ex=RedisConfig.expire)
print(redis.get("first"))