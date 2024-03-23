FROM bitnami/mariadb-galera:latest

USER root

RUN mkdir -p /var/lib/apt/lists/partial && \
    apt-get clean && \
    apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y sudo 

# Copy the script into the container
COPY bootstrap.sh /bootstrap.sh

ENTRYPOINT ["/bootstrap.sh"]
