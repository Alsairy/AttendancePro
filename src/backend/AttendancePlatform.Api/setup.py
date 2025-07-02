from setuptools import setup, find_packages

setup(
    name="attendanceplatform-api",
    version="0.1.0",
    description="Hudur Enterprise Platform Backend API",
    author="Devin AI",
    author_email="devin-ai-integration[bot]@users.noreply.github.com",
    packages=find_packages(),
    install_requires=[
        "fastapi==0.104.1",
        "uvicorn[standard]==0.24.0",
        "python-multipart==0.0.6",
        "pyjwt==2.8.0",
        "bcrypt==4.1.2"
    ],
    entry_points={
        "console_scripts": [
            "start=uvicorn app.main:app --host 0.0.0.0 --port 8080",
        ],
    },
)
