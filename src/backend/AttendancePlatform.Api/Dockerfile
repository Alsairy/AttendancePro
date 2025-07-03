FROM python:3.11-slim

WORKDIR /app

# Install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy application code
COPY app/ ./app/

# Create a startup script to avoid auto-detection
RUN echo '#!/bin/bash\npython -m uvicorn app.main:app --host 0.0.0.0 --port 8080' > /app/start.sh && chmod +x /app/start.sh

# Expose port
EXPOSE 8080

# Start the application using the script
CMD ["/app/start.sh"]
