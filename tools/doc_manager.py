import sqlite3
import os
import datetime
import argparse
import sys

# Determine the directory where this script is located
BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DB_NAME = os.path.join(BASE_DIR, 'doc_system.db')

def init_db():
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    
    # 强制重建表，开发阶段方便调试
    # c.execute('DROP TABLE IF EXISTS documents')
    # c.execute('DROP TABLE IF EXISTS history')
    # c.execute('DROP TABLE IF EXISTS qa_history')
    # c.execute('DROP TABLE IF EXISTS developers')
    
    # 文档表
    c.execute('''CREATE TABLE IF NOT EXISTS documents (
        id TEXT PRIMARY KEY,
        level INTEGER,
        parent_id TEXT,
        filename TEXT,
        title TEXT,
        creator TEXT,
        create_time TEXT,
        path TEXT,
        FOREIGN KEY(parent_id) REFERENCES documents(id)
    )''')
    
    # 历史记录表
    c.execute('''CREATE TABLE IF NOT EXISTS history (
        history_id INTEGER PRIMARY KEY AUTOINCREMENT,
        doc_id TEXT,
        action TEXT,
        timestamp TEXT,
        operator TEXT,
        detail TEXT,
        FOREIGN KEY(doc_id) REFERENCES documents(id)
    )''')

    # AI 问答记录表
    c.execute('''CREATE TABLE IF NOT EXISTS qa_history (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        question TEXT,
        answer TEXT,
        timestamp TEXT,
        context_files TEXT
    )''')

    # 开发人员表
    c.execute('''CREATE TABLE IF NOT EXISTS developers (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT,
        role TEXT
    )''')
    
    # 用户表
    c.execute('''CREATE TABLE IF NOT EXISTS users (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        username TEXT UNIQUE NOT NULL,
        password_hash TEXT NOT NULL,
        role TEXT DEFAULT 'user',
        created_at TEXT
    )''')
    
    # 组织表
    c.execute('''CREATE TABLE IF NOT EXISTS organizations (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        parent_id INTEGER,
        manager_id INTEGER,
        created_at TEXT,
        FOREIGN KEY(parent_id) REFERENCES organizations(id),
        FOREIGN KEY(manager_id) REFERENCES users(id)
    )''')
    
    # 组织成员表
    c.execute('''CREATE TABLE IF NOT EXISTS organization_members (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        org_id INTEGER NOT NULL,
        user_id INTEGER NOT NULL,
        role TEXT DEFAULT 'member',
        joined_at TEXT,
        FOREIGN KEY(org_id) REFERENCES organizations(id),
        FOREIGN KEY(user_id) REFERENCES users(id),
        UNIQUE(org_id, user_id)
    )''')
    
    # 任务管理表
    c.execute('''CREATE TABLE IF NOT EXISTS tasks (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        parent_id INTEGER,
        title TEXT NOT NULL,
        detail TEXT,
        assignee TEXT,
        status TEXT CHECK(status IN ('pending', 'in_progress', 'completed', 'blocked')) DEFAULT 'pending',
        priority INTEGER DEFAULT 1,
        doc_id TEXT,
        org_id INTEGER,
        created_at TEXT,
        updated_at TEXT,
        FOREIGN KEY(parent_id) REFERENCES tasks(id),
        FOREIGN KEY(org_id) REFERENCES organizations(id)
    )''')
    
    conn.commit()
    conn.close()
    print(f"Database {DB_NAME} initialized/checked.")

def register_document(id, level, parent_id, filename, title, creator, path, create_time=None):
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    
    now = datetime.datetime.now().strftime("%Y%m%d%H%M%S")
    if not create_time:
        # Default create time logic if not provided
        try:
             create_time = filename.split('_time_')[1].split('_')[0] 
        except:
             create_time = now
    
    # Check if exists
    c.execute("SELECT id FROM documents WHERE id=?", (id,))
    exists = c.fetchone()
    
    if exists:
        # Update
        c.execute('''UPDATE documents SET 
            level=?, parent_id=?, filename=?, title=?, path=?
            WHERE id=?''', (level, parent_id, filename, title, path, id))
        action = "UPDATE"
        print(f"Document {id} updated.")
    else:
        # Insert
        c.execute('''INSERT INTO documents 
            (id, level, parent_id, filename, title, creator, create_time, path)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)''', 
            (id, level, parent_id, filename, title, creator, create_time, path))
        action = "CREATE"
        print(f"Document {id} created.")
        
    # Log history
    c.execute('''INSERT INTO history (doc_id, action, timestamp, operator, detail)
        VALUES (?, ?, ?, ?, ?)''', 
        (id, action, now, creator, f"Filename: {filename}"))
    
    conn.commit()
    conn.close()

def list_documents(parent_id=None):
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    if parent_id:
        c.execute("SELECT id, title, filename FROM documents WHERE parent_id=?", (parent_id,))
    else:
        c.execute("SELECT id, title, filename, parent_id FROM documents")
    
    rows = c.fetchall()
    print(f"{'ID':<10} {'Parent':<10} {'Title':<30} {'Filename'}")
    print("-" * 80)
    for row in rows:
        pid = row[3] if len(row) > 3 else parent_id
        print(f"{row[0]:<10} {pid:<10} {row[1]:<30} {row[2]}")
    conn.close()

def parse_filename(file_path):
    """
    Parses filename format: Name_id_ID_level_LEVEL_parentId_PID_time_TIME_Creator_CREATOR.md
    """
    try:
        # Normalize path separators
        file_path = file_path.replace('\\', '/')
        
        # Split into directory and filename
        directory = os.path.dirname(file_path)
        filename = os.path.basename(file_path)
        
        # If directory is empty, assume it's current directory or root depending on context.
        # But for the DB, we want the path relative to project root.
        # If the script is run from project root, directory is correct.
        if not directory:
            directory = './'

        # Handle Windows CLI encoding issues aggressively
        filename_clean = filename
        try:
            # If we see the typical mojibake, try to re-encode/decode if possible, 
            # OR just be more robust in finding the keywords.
            pass
        except:
            pass
            
        parts = filename_clean.replace('.md', '').split('_')
        info = {}
        info['filename'] = filename
        info['path'] = directory
        
        # Robust parsing: look for keywords regardless of position
        for i, part in enumerate(parts):
            # Check for combined mojibake like '..._id'
            if part == 'id' or part.endswith('id'): 
                if i+1 < len(parts): info['id'] = parts[i+1]
            if part == 'level' and i+1 < len(parts): info['level'] = int(parts[i+1])
            if part == 'parentId' and i+1 < len(parts): info['parent_id'] = parts[i+1]
            if part == 'time' and i+1 < len(parts): info['create_time'] = parts[i+1]
            if part == 'Creator' and i+1 < len(parts): info['creator'] = parts[i+1]
            
        # Try to clean up title if it's garbage
        info['title'] = parts[0]
        
        # Validation
        if 'id' not in info:
            # Fallback debug
            print(f"Debug: Parts found -> {parts}")
            return None
            
        return info
    except Exception as e:
        print(f"Error parsing filename: {e}")
        return None

def log_qa(question, answer, context_files=""):
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    now = datetime.datetime.now().strftime("%Y%m%d%H%M%S")
    c.execute("INSERT INTO qa_history (question, answer, timestamp, context_files) VALUES (?, ?, ?, ?)",
              (question, answer, now, context_files))
    conn.commit()
    conn.close()
    print("QA log saved.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Document Management Tool')
    subparsers = parser.add_subparsers(dest='command')
    
    # Init
    parser_init = subparsers.add_parser('init', help='Initialize database')
    
    # Add/Update
    parser_add = subparsers.add_parser('register', help='Register or update a document')
    parser_add.add_argument('filename', help='Filename to register')
    
    # List
    parser_list = subparsers.add_parser('list', help='List documents')
    parser_list.add_argument('--parent', help='Filter by parent ID')

    # QA Log
    parser_qa = subparsers.add_parser('log_qa', help='Log AI QA interaction')
    parser_qa.add_argument('question', help='User Question')
    parser_qa.add_argument('answer', help='AI Answer')
    parser_qa.add_argument('--context', help='Context files', default="")

    args = parser.parse_args()
    
    if args.command == 'init':
        init_db()
    elif args.command == 'register':
        if not os.path.exists(DB_NAME): init_db()
        # On Windows CMD, args might be in system encoding. Python 3 usually handles this well,
        # but if we see mojibake, we might need manual handling.
        # For now, rely on Python's auto handling.
        info = parse_filename(args.filename)
        if info:
            register_document(
                id=info.get('id'),
                level=info.get('level'),
                parent_id=info.get('parent_id'),
                filename=info.get('filename'),
                title=info.get('title'),
                creator=info.get('creator'),
                path=info.get('path')
            )
        else:
            print("Failed to parse info from filename")
    elif args.command == 'list':
        list_documents(args.parent)
    elif args.command == 'log_qa':
        if not os.path.exists(DB_NAME): init_db()
        log_qa(args.question, args.answer, args.context)
    else:
        # Default behavior if no args: init and register root doc (for convenience in this session)
        if not os.path.exists(DB_NAME):
             init_db()
        
        doc_path = "总框架_id_00000000_level_0_parentId_null_time_20251218_Creator_lvwan01.md"
        if os.path.exists(doc_path):
             print("Auto-registering root document...")
             info = parse_filename(doc_path)
             if info:
                register_document(**info)
        else:
            parser.print_help()
