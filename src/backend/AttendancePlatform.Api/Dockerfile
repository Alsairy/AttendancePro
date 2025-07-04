FROM python:3.11-slim

WORKDIR /app

# Install dependencies

COPY requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

# Copy application code
<<<<<<< HEAD
COPY comprehensive_services.py ./
||||||| 2945096
COPY app/ ./app/
=======

COPY comprehensive_services.py ./
>>>>>>> eed39a60f6b5e10c305cb9316422288e04759c18

# Expose port

EXPOSE 8080

<<<<<<< HEAD
# Use direct python execution to avoid any auto-detection
CMD ["python", "comprehensive_services.py"]
||||||| 2945096
# Start the application with uvicorn
CMD ["python", "-m", "uvicorn", "app.server:application", "--host", "0.0.0.0", "--port", "8080"]
=======
# Use direct python execution to avoid any auto-detection

CMD ["python", "comprehensive_services.py"]
>>>>>>> eed39a60f6b5e10c305cb9316422288e04759c18
