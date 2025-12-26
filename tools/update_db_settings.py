import sqlite3
import os

DB_NAME = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'doc_system.db')

def update_db():
    print(f"Connecting to {DB_NAME}...")
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    
    # Check if table exists
    c.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='settings'")
    if c.fetchone():
        print("Table 'settings' already exists.")
    else:
        print("Creating table 'settings'...")
        c.execute('''CREATE TABLE settings (
            key TEXT PRIMARY KEY,
            value TEXT
        )''')
        print("Table created.")

    conn.commit()
    conn.close()

if __name__ == "__main__":
    update_db()

