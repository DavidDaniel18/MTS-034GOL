#!/bin/bash

INITIALIZED_MARKER="/bitnami/mariadb/init/.initialized"

echo "Checking if the cluster has been previously initialized..."

# Determine if we need to bootstrap the cluster
if [ -f "$INITIALIZED_MARKER" ]; then
    # This file exists if the cluster has been initialized before
    BOOTSTRAP="no"
    echo "Initialization marker found. This node will not bootstrap the cluster."
else
    # No flag file, must be the first start
    BOOTSTRAP="yes"
    echo "Initialization marker not found. This node will bootstrap the cluster."
    touch "$INITIALIZED_MARKER"
    echo "Initialization marker created at $INITIALIZED_MARKER"
fi

echo "Setting MARIADB_GALERA_CLUSTER_BOOTSTRAP to '$BOOTSTRAP'."

# Now start MariaDB based on the BOOTSTRAP value
if [ "$BOOTSTRAP" = "yes" ]; then
    export MARIADB_GALERA_CLUSTER_BOOTSTRAP="yes"
else
    export MARIADB_GALERA_CLUSTER_BOOTSTRAP="no"
fi

echo "Starting MariaDB..."
exec /opt/bitnami/scripts/mariadb-galera/entrypoint.sh /opt/bitnami/scripts/mariadb-galera/run.sh "$@"
