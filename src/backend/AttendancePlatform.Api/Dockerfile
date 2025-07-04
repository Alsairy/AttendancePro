FROM python:3.11-slim

WORKDIR /app

# Install dependencies

COPY requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

# Copy application code

COPY comprehensive_services.py ./

# Expose port

EXPOSE 8080

# Use direct python execution to avoid any auto-detection

CMD ["python", "comprehensive_services.py"]
