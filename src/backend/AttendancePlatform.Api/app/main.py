import uvicorn
import os
import sys

sys.path.insert(0, '/app')

def start():
    """Start the FastAPI application with uvicorn"""
    try:
        from app.server import application
        port = int(os.environ.get("PORT", 8080))
        print(f"Starting Hudur Enterprise Platform on port {port}")
        uvicorn.run(
            application, 
            host="0.0.0.0", 
            port=port,
            log_level="info"
        )
    except Exception as e:
        print(f"Failed to start application: {e}")
        sys.exit(1)

app = application

if __name__ == "__main__":
    start()
