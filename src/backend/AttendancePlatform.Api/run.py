#!/usr/bin/env python3
import uvicorn
import os

def main():
    from app.server import application
    port = int(os.environ.get("PORT", 8080))
    uvicorn.run(application, host="0.0.0.0", port=port)

if __name__ == "__main__":
    main()
