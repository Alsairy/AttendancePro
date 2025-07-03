FROM python:3.11-slim

WORKDIR /app

# Install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy application code
COPY app/ ./app/
COPY run.py ./

# Expose port
EXPOSE 8080

# Start the application with python run.py to avoid FastAPI CLI detection
CMD ["python", "run.py"]
