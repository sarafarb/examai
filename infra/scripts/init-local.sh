#!/bin/bash
set -e

echo "🚀 Starting local development environment..."
cd "$(dirname "$0")/../docker"

# Load environment variables explicitly for the compose setup
docker compose --env-file .env.local up -d

echo "⏳ Waiting for PostgreSQL to be ready..."
until docker exec examai_postgres pg_isready -U examai > /dev/null 2>&1; do
  sleep 1
done
echo "✅ PostgreSQL is ready!"

echo "🔄 Running initial database migrations..."
# Placeholder for future migrations (e.g. dotnet ef database update)
echo "No migrations configured yet. Skipping."

echo "📦 Creating MinIO bucket 'examai-files'..."
# Use temporary minio client container attached to local network to create the bucket
docker run --rm --network host minio/mc:RELEASE.2024-01-11T05-52-11Z \
  sh -c "mc alias set local http://127.0.0.1:9000 examai examai_local && mc mb --ignore-existing local/examai-files"
echo "✅ Bucket 'examai-files' created successfully!"

echo "--------------------------------------------------"
echo "🎯 Local Development Environment Setup Complete!"
echo "--------------------------------------------------"
echo "📊 PostgreSQL:      localhost:5432  (User: examai)"
echo "🍒 Redis:           localhost:6379"
echo "🐇 RabbitMQ Mgmt:   http://localhost:15672 (User: examai)"
echo "🪣 MinIO Console:   http://localhost:9001  (User: examai)"
echo "🔍 Elasticsearch:  http://localhost:9200"
echo "📋 Seq Log Viewer:  http://localhost:5341"
echo "🛡️ ClamAV Daemon:   localhost:3310"
echo "--------------------------------------------------"
