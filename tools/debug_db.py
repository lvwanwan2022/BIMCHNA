import sqlite3
import os

DB_NAME = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), 'doc_system.db')
print(f"Checking DB at: {DB_NAME}")

if os.path.exists(DB_NAME):
    print("File exists.")
    try:
        conn = sqlite3.connect(DB_NAME)
        cursor = conn.execute("SELECT name FROM sqlite_master WHERE type='table';")
        tables = cursor.fetchall()
        print("Tables:", [t[0] for t in tables])
        
        if 'documents' in [t[0] for t in tables]:
            count = conn.execute("SELECT count(*) FROM documents").fetchone()[0]
            print(f"Documents count: {count}")
            
        conn.close()
    except Exception as e:
        print(f"Error reading DB: {e}")
else:
    print("File does not exist.")

