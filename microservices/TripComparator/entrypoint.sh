#!/bin/sh
chown -R app:app /app/data
su - app
exec "$@"
