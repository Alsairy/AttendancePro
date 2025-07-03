import uvicorn
import os
import sys

sys.path.insert(0, '/app')

def start_application():
    try:
        from app.server import application
        port = int(os.environ.get("PORT", 8080))
        print(f"Starting server on port {port}")
        uvicorn.run(application, host="0.0.0.0", port=port)
    except Exception as e:
        print(f"Error starting application: {e}")
        sys.exit(1)

if __name__ == "__main__":
    start_application()
