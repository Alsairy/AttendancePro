#!/usr/bin/env python3
import uvicorn
from app.server import application

if __name__ == "__main__":
    import os
    port = int(os.environ.get("PORT", 8080))
    uvicorn.run(application, host="0.0.0.0", port=port)
