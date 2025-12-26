import sqlite3
import os

DB_NAME = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'doc_system.db')

def update_db():
    print(f"Connecting to {DB_NAME}...")
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    
    # Check if table exists
    c.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='tasks'")
    if c.fetchone():
        print("Table 'tasks' already exists.")
    else:
        print("Creating table 'tasks'...")
        c.execute('''CREATE TABLE tasks (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            parent_id INTEGER,
            title TEXT NOT NULL,
            detail TEXT,
            assignee TEXT,
            status TEXT CHECK(status IN ('pending', 'in_progress', 'completed', 'blocked')) DEFAULT 'pending',
            priority INTEGER DEFAULT 1,
            created_at TEXT,
            updated_at TEXT,
            FOREIGN KEY(parent_id) REFERENCES tasks(id)
        )''')
        print("Table created.")

    conn.commit()
    conn.close()

if __name__ == "__main__":
    update_db()

