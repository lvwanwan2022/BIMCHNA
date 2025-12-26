from flask import Flask, jsonify, request, send_from_directory, send_file, Response, stream_with_context
import sqlite3
import os
import datetime
import requests
import json
from werkzeug.security import generate_password_hash, check_password_hash

app = Flask(__name__)
app.config['JSON_AS_ASCII'] = False
# Database is in the project root, one level up from this script
PROJECT_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DB_NAME = os.path.join(PROJECT_ROOT, 'doc_system.db')
print(f"Using database at: {DB_NAME}")

def validate_path_in_project(path_str, is_directory=False, allow_create=False):
    """
    Validates that a path is within the project root.
    Returns (full_path, error_message).
    """
    if not path_str:
        return None, None
        
    # Handle both absolute and relative paths
    if os.path.isabs(path_str):
        # If absolute, check if it starts with project root
        try:
            if not os.path.commonpath([PROJECT_ROOT, os.path.abspath(path_str)]) == PROJECT_ROOT:
                 return None, "Path must be inside project directory"
            full_path = path_str
        except ValueError:
             # Can happen on Windows if drives are different
             return None, "Path must be on the same drive as project directory"
    else:
        # If relative, join with project root
        full_path = os.path.join(PROJECT_ROOT, path_str)
        
    # Normalize path
    full_path = os.path.normpath(full_path)
    
    # Re-check security after normalization to prevent '..' traversal
    if not full_path.startswith(PROJECT_ROOT):
        return None, "Path must be inside project directory"
        
    # Check existence
    if is_directory:
        if not os.path.exists(full_path):
            if allow_create:
                try:
                    os.makedirs(full_path, exist_ok=True)
                except Exception as e:
                    return None, f"Failed to create directory: {str(e)}"
            else:
                return None, f"Directory does not exist: {path_str}"
        elif not os.path.isdir(full_path):
             return None, f"Path exists but is not a directory: {path_str}"
    else:
        # For files, check if parent directory exists
        parent_dir = os.path.dirname(full_path)
        if not os.path.exists(parent_dir):
            if allow_create:
                try:
                    os.makedirs(parent_dir, exist_ok=True)
                except Exception as e:
                    return None, f"Failed to create parent directory: {str(e)}"
            else:
                return None, f"Parent directory does not exist for: {path_str}"
             
    return full_path, None

def get_db_connection():
    conn = sqlite3.connect(DB_NAME)
    conn.row_factory = sqlite3.Row
    return conn

@app.route('/')
def index():
    dashboard_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'dashboard.html')
    return send_file(dashboard_path)

@app.route('/api/documents')
def get_documents():
    conn = get_db_connection()
    docs = conn.execute('SELECT * FROM documents').fetchall()
    conn.close()
    return jsonify([dict(doc) for doc in docs])

@app.route('/api/qa')
def get_qa():
    conn = get_db_connection()
    qa_list = conn.execute('SELECT * FROM qa_history ORDER BY timestamp DESC').fetchall()
    conn.close()
    return jsonify([dict(row) for row in qa_list])

@app.route('/api/history')
def get_history():
    conn = get_db_connection()
    history = conn.execute('SELECT * FROM history ORDER BY timestamp DESC').fetchall()
    conn.close()
    return jsonify([dict(row) for row in history])

@app.route('/api/doc_content/<path:doc_id>')
def get_doc_content(doc_id):
    conn = get_db_connection()
    doc = conn.execute('SELECT * FROM documents WHERE id = ?', (doc_id,)).fetchone()
    conn.close()
    
    if not doc:
        return jsonify({'error': 'Document not found'}), 404
        
    project_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    full_path = os.path.join(project_root, doc['path'], doc['filename'])
    
    # Special handling for root doc which is in project root
    if doc['level'] == 0:
         full_path = os.path.join(project_root, doc['filename'])

    try:
        with open(full_path, 'r', encoding='utf-8') as f:
            content = f.read()
        return jsonify({'content': content, 'full_path': full_path})
    except Exception as e:
        return jsonify({'error': f"File read error: {str(e)}", 'path': full_path}), 404

@app.route('/api/files/serve', methods=['GET'])
def serve_static_file():
    path_str = request.args.get('path')
    if not path_str:
        return jsonify({'error': 'Path is required'}), 400
        
    full_path, error = validate_path_in_project(path_str, is_directory=False, allow_create=False)
    
    if error:
        return jsonify({'error': error}), 403
        
    if not os.path.exists(full_path):
        return jsonify({'error': 'File not found'}), 404
        
    return send_file(full_path)

# --- Tasks API ---
@app.route('/api/tasks', methods=['GET'])
def get_tasks():
    conn = get_db_connection()
    tasks = conn.execute('SELECT * FROM tasks').fetchall()
    conn.close()
    return jsonify([dict(row) for row in tasks])

@app.route('/api/tasks', methods=['POST'])
def create_task():
    data = request.json
    title = data.get('title')
    if not title:
        return jsonify({'error': 'Title is required'}), 400
        
    conn = get_db_connection()
    now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    cursor = conn.execute(
        'INSERT INTO tasks (parent_id, title, detail, assignee, status, priority, doc_id, org_id, created_at, updated_at) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)',
        (data.get('parent_id'), title, data.get('detail'), data.get('assignee'), data.get('status', 'pending'), data.get('priority', 1), data.get('doc_id'), data.get('org_id'), now, now)
    )
    conn.commit()
    new_id = cursor.lastrowid
    conn.close()
    return jsonify({'id': new_id, 'status': 'success'}), 201

@app.route('/api/tasks/<int:task_id>', methods=['PUT'])
def update_task(task_id):
    data = request.json
    conn = get_db_connection()
    now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    
    fields = []
    values = []
    for key in ['parent_id', 'title', 'detail', 'assignee', 'status', 'priority', 'doc_id', 'org_id']:
        if key in data:
            fields.append(f"{key} = ?")
            values.append(data[key])
    
    if not fields:
        return jsonify({'error': 'No fields to update'}), 400
        
    fields.append("updated_at = ?")
    values.append(now)
    values.append(task_id)
    
    conn.execute(f'UPDATE tasks SET {", ".join(fields)} WHERE id = ?', values)
    conn.commit()
    conn.close()
    return jsonify({'status': 'success'})

@app.route('/api/tasks/<int:task_id>', methods=['DELETE'])
def delete_task(task_id):
    conn = get_db_connection()
    conn.execute('DELETE FROM tasks WHERE id = ?', (task_id,))
    conn.commit()
    conn.close()
    return jsonify({'status': 'success'})

# --- Organization API ---
@app.route('/api/orgs', methods=['GET'])
def get_orgs():
    conn = get_db_connection()
    orgs = conn.execute('''
        SELECT o.*, u.username as manager_name 
        FROM organizations o 
        LEFT JOIN users u ON o.manager_id = u.id
    ''').fetchall()
    conn.close()
    return jsonify([dict(row) for row in orgs])

@app.route('/api/orgs', methods=['POST'])
def create_org():
    data = request.json
    name = data.get('name')
    if not name: return jsonify({'error': 'Name is required'}), 400
    
    conn = get_db_connection()
    now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    
    try:
        cursor = conn.execute('INSERT INTO organizations (name, parent_id, manager_id, created_at) VALUES (?, ?, ?, ?)',
                     (name, data.get('parent_id'), data.get('manager_id'), now))
        org_id = cursor.lastrowid
        
        # If manager is set, add to members automatically
        manager_id = data.get('manager_id')
        if manager_id:
            conn.execute('INSERT OR IGNORE INTO organization_members (org_id, user_id, role, joined_at) VALUES (?, ?, ?, ?)',
                         (org_id, manager_id, 'manager', now))
                         
        conn.commit()
        conn.close()
        return jsonify({'status': 'success', 'id': org_id})
    except Exception as e:
        conn.close()
        return jsonify({'error': str(e)}), 500

@app.route('/api/orgs/<int:org_id>/members', methods=['GET'])
def get_org_members(org_id):
    conn = get_db_connection()
    members = conn.execute('''
        SELECT u.id, u.username, om.role, om.joined_at 
        FROM organization_members om
        JOIN users u ON om.user_id = u.id
        WHERE om.org_id = ?
    ''', (org_id,)).fetchall()
    conn.close()
    return jsonify([dict(row) for row in members])

@app.route('/api/orgs/<int:org_id>/members', methods=['POST'])
def add_org_member(org_id):
    data = request.json
    user_id = data.get('user_id')
    if not user_id: return jsonify({'error': 'User ID required'}), 400
    
    conn = get_db_connection()
    now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    try:
        conn.execute('INSERT INTO organization_members (org_id, user_id, role, joined_at) VALUES (?, ?, ?, ?)',
                     (org_id, user_id, data.get('role', 'member'), now))
        conn.commit()
        conn.close()
        return jsonify({'status': 'success'})
    except sqlite3.IntegrityError:
        conn.close()
        return jsonify({'error': 'User already in organization'}), 400

# --- File Editor API ---
@app.route('/api/file', methods=['POST'])
def save_file():
    data = request.json
    # Allow saving via Doc ID or direct path
    
    doc_id = data.get('doc_id')
    file_path = data.get('file_path')
    content = data.get('content')
    
    if (not doc_id and not file_path) or content is None:
        return jsonify({'error': 'Missing doc_id/file_path or content'}), 400
        
    conn = get_db_connection()
    full_path = ""
    
    if doc_id:
        doc = conn.execute('SELECT * FROM documents WHERE id = ?', (doc_id,)).fetchone()
        
        if not doc:
            conn.close()
            return jsonify({'error': 'Document not found'}), 404
            
        full_path = os.path.join(PROJECT_ROOT, doc['path'], doc['filename'])
        if doc['level'] == 0:
             full_path = os.path.join(PROJECT_ROOT, doc['filename'])
    elif file_path:
        # Use new validation logic
        validated_path, error = validate_path_in_project(file_path, is_directory=False, allow_create=True)
        if error:
            conn.close()
            return jsonify({'error': error}), 400
        full_path = validated_path
             
    conn.close()
         
    try:
        with open(full_path, 'w', encoding='utf-8') as f:
            f.write(content)
        return jsonify({'status': 'success'})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

# --- File System API ---
@app.route('/api/fs/suggestions', methods=['GET'])
def fs_suggestions():
    query = request.args.get('query', '')
    
    # Normalize query separators
    query = query.replace('\\', '/')
    
    # Determine base directory and partial match
    if query.endswith('/'):
        search_dir = query
        partial = ""
    else:
        search_dir = os.path.dirname(query)
        partial = os.path.basename(query)
        
    # Resolve path relative to PROJECT_ROOT
    full_search_dir, error = validate_path_in_project(search_dir if search_dir else '.', is_directory=True, allow_create=False)
    
    if error or not full_search_dir:
        return jsonify([])

    suggestions = []
    try:
        with os.scandir(full_search_dir) as entries:
            for entry in entries:
                # Filter by partial
                if partial and not entry.name.lower().startswith(partial.lower()):
                    continue
                
                # Filter hidden files/dirs
                if entry.name.startswith('.') or entry.name == '__pycache__' or entry.name == 'node_modules':
                    continue
                    
                # Format result
                rel_path = os.path.join(search_dir, entry.name).replace('\\', '/')
                if entry.is_dir():
                    rel_path += '/'
                    
                suggestions.append({
                    'value': rel_path,
                    'text': entry.name + ('/' if entry.is_dir() else ''),
                    'type': 'dir' if entry.is_dir() else 'file'
                })
    except Exception as e:
        print(f"Error listing dir {full_search_dir}: {e}")
        return jsonify([])
        
    # Sort: directories first, then files
    suggestions.sort(key=lambda x: (x['type'] != 'dir', x['text'].lower()))
    
    return jsonify(suggestions)

# --- Settings API ---
@app.route('/api/settings', methods=['GET'])
def get_settings():
    conn = get_db_connection()
    settings = conn.execute('SELECT * FROM settings').fetchall()
    conn.close()
    return jsonify({row['key']: row['value'] for row in settings})

@app.route('/api/settings', methods=['POST'])
def update_settings():
    data = request.json
    conn = get_db_connection()
    
    for key, value in data.items():
        # Upsert
        conn.execute('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', (key, value))
        
    conn.commit()
    conn.close()
    return jsonify({'status': 'success'})

# --- DeepSeek Proxy ---
@app.route('/api/ai/chat', methods=['POST'])
def ai_chat():
    data = request.json
    messages = data.get('messages')
    
    # Get Settings from DB
    conn = get_db_connection()
    rows = conn.execute("SELECT key, value FROM settings").fetchall()
    conn.close()
    
    settings = {row['key']: row['value'] for row in rows}
    api_key = settings.get('deepseek_api_key')
    base_url = settings.get('ai_base_url', 'https://api.deepseek.com')
    model = settings.get('ai_model', 'deepseek-chat')
    
    if not api_key:
        return jsonify({'error': '请先在设置中配置 DeepSeek API Key'}), 400

    # Call DeepSeek API
    def generate():
        try:
            headers = {
                'Content-Type': 'application/json',
                'Authorization': f'Bearer {api_key}'
            }
            payload = {
                'model': model,
                'messages': messages,
                'temperature': 0.7,
                'stream': True
            }
            
            # Ensure base_url doesn't end with slash
            base = base_url.rstrip('/')
            # Handle if user input full path or just base
            if not base.endswith('/chat/completions'):
                 endpoint = f"{base}/chat/completions"
            else:
                 endpoint = base
            
            with requests.post(endpoint, headers=headers, json=payload, stream=True, timeout=60) as response:
                if response.status_code != 200:
                    yield f"data: {json.dumps({'error': f'AI Provider Error: {response.text}'})}\n\n"
                    return

                for line in response.iter_lines():
                    if line:
                        decoded_line = line.decode('utf-8')
                        if decoded_line.startswith('data: '):
                            yield f"{decoded_line}\n\n"
                            
        except Exception as e:
            yield f"data: {json.dumps({'error': str(e)})}\n\n"

    return Response(stream_with_context(generate()), mimetype='text/event-stream')

# --- Database Viewer API ---
@app.route('/api/db/tables', methods=['GET'])
def get_tables():
    conn = get_db_connection()
    tables = conn.execute("SELECT name FROM sqlite_master WHERE type='table'").fetchall()
    conn.close()
    return jsonify([row['name'] for row in tables])

@app.route('/api/db/query', methods=['POST'])
def query_db():
    data = request.json
    sql = data.get('sql')
    
    if not sql:
        return jsonify({'error': 'SQL query is required'}), 400
        
    # Security check: Only allow SELECT
    if not sql.strip().upper().startswith('SELECT'):
        return jsonify({'error': 'Only SELECT queries are allowed via this interface.'}), 403

    conn = get_db_connection()
    try:
        cursor = conn.execute(sql)
        rows = cursor.fetchall()
        
        # Get headers
        headers = [description[0] for description in cursor.description] if cursor.description else []
        
        # Convert rows to dicts
        results = [dict(row) for row in rows]
        
        conn.close()
        return jsonify({'headers': headers, 'rows': results})
    except Exception as e:
        conn.close()
        return jsonify({'error': str(e)}), 400

@app.route('/api/db/schema/<table_name>', methods=['GET'])
def get_table_schema(table_name):
    conn = get_db_connection()
    try:
        # Use PRAGMA table_info to get schema even if table is empty
        cursor = conn.execute(f"PRAGMA table_info({table_name})")
        rows = cursor.fetchall()
        
        # PRAGMA returns: cid, name, type, notnull, dflt_value, pk
        headers = ['cid', 'name', 'type', 'notnull', 'dflt_value', 'pk']
        results = [dict(row) for row in rows]
        
        conn.close()
        return jsonify({'headers': headers, 'rows': results})
    except Exception as e:
        conn.close()
        return jsonify({'error': str(e)}), 400

# --- Auth API ---
@app.route('/api/auth/register', methods=['POST'])
def register():
    data = request.json
    username = data.get('username')
    password = data.get('password')
    user_type = data.get('type', 'human') # Default to human
    
    if not username or not password:
        return jsonify({'error': 'Username and password are required'}), 400
        
    conn = get_db_connection()
    
    # Check if user exists
    existing = conn.execute('SELECT id FROM users WHERE username = ?', (username,)).fetchone()
    if existing:
        conn.close()
        return jsonify({'error': 'Username already exists'}), 400
        
    password_hash = generate_password_hash(password)
    now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    
    try:
        cursor = conn.execute('INSERT INTO users (username, password_hash, role, type, created_at) VALUES (?, ?, ?, ?, ?)',
                     (username, password_hash, 'user', user_type, now))
        user_id = cursor.lastrowid
        
        # If type is agent, create agent entry
        if user_type == 'ai_agent':
            conn.execute('INSERT INTO ai_agents (user_id, target) VALUES (?, ?)', (user_id, ''))
            
        conn.commit()
        conn.close()
        return jsonify({'status': 'success'})
    except Exception as e:
        conn.close()
        return jsonify({'error': str(e)}), 500

@app.route('/api/auth/login', methods=['POST'])
def login():
    data = request.json
    username = data.get('username')
    password = data.get('password')
    
    if not username or not password:
        return jsonify({'error': 'Username and password are required'}), 400
        
    conn = get_db_connection()
    user = conn.execute('SELECT * FROM users WHERE username = ?', (username,)).fetchone()
    
    if user and check_password_hash(user['password_hash'], password):
        agent_info = None
        if user['type'] == 'ai_agent':
            agent = conn.execute('SELECT * FROM ai_agents WHERE user_id = ?', (user['id'],)).fetchone()
            if agent:
                agent_info = dict(agent)
                
        conn.close()
        
        return jsonify({
            'status': 'success',
            'user': {
                'id': user['id'],
                'username': user['username'],
                'role': user['role'],
                'type': user['type']
            },
            'agent': agent_info
        })
    else:
        conn.close()
        return jsonify({'error': 'Invalid username or password'}), 401

# --- Users API ---
@app.route('/api/users', methods=['GET'])
def get_users():
    conn = get_db_connection()
    users = conn.execute('SELECT id, username, role, type FROM users').fetchall()
    conn.close()
    return jsonify([dict(row) for row in users])

# --- Agents API ---
@app.route('/api/agents/<int:user_id>/info', methods=['GET'])
def get_agent_info(user_id):
    conn = get_db_connection()
    agent = conn.execute('SELECT * FROM ai_agents WHERE user_id = ?', (user_id,)).fetchone()
    conn.close()
    if agent:
        return jsonify(dict(agent))
    return jsonify({'error': 'Agent not found'}), 404

@app.route('/api/agents/<int:user_id>/update', methods=['POST'])
def update_agent_info(user_id):
    data = request.json
    conn = get_db_connection()
    
    # Check if agent exists
    existing = conn.execute('SELECT id FROM ai_agents WHERE user_id = ?', (user_id,)).fetchone()
    if not existing:
         conn.execute('INSERT INTO ai_agents (user_id) VALUES (?)', (user_id,))
    
    # Validate paths if provided
    if 'learning_materials_path' in data and data['learning_materials_path']:
        path_val = data['learning_materials_path']
        full_path, error = validate_path_in_project(path_val, is_directory=True, allow_create=True)
        if error:
            conn.close()
            return jsonify({'error': f"Invalid learning path: {error}"}), 400
            
    if 'operable_files' in data and data['operable_files']:
        path_val = data['operable_files']
        # Heuristic: if no extension, assume directory for "create file in folder" mode
        _, ext = os.path.splitext(path_val)
        is_dir = not ext
        
        full_path, error = validate_path_in_project(path_val, is_directory=is_dir, allow_create=True)
        if error:
            conn.close()
            return jsonify({'error': f"Invalid operable file/path: {error}"}), 400

    fields = []
    values = []
    for key in ['target', 'learning_materials_path', 'operable_files', 'important_knowledge']:
        if key in data:
            fields.append(f"{key} = ?")
            values.append(data[key] if data[key] else '') # Handle nulls
            
    if fields:
        values.append(user_id)
        conn.execute(f'UPDATE ai_agents SET {", ".join(fields)} WHERE user_id = ?', values)
        conn.commit()
        
    conn.close()
    return jsonify({'status': 'success'})

@app.route('/api/agents/<int:user_id>/learn', methods=['POST'])
def agent_learn(user_id):
    data = request.json
    content = data.get('content')
    source = data.get('source', 'manual')
    
    conn = get_db_connection()
    agent = conn.execute('SELECT id FROM ai_agents WHERE user_id = ?', (user_id,)).fetchone()
    if not agent:
        conn.close()
        return jsonify({'error': 'Agent not found'}), 404
        
    now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    conn.execute('INSERT INTO agent_learning_records (agent_id, content, source, timestamp) VALUES (?, ?, ?, ?)',
                 (agent['id'], content, source, now))
    conn.commit()
    conn.close()
    return jsonify({'status': 'success'})

@app.route('/api/agents/<int:user_id>/learning_history', methods=['GET'])
def get_agent_learning_history(user_id):
    conn = get_db_connection()
    agent = conn.execute('SELECT id FROM ai_agents WHERE user_id = ?', (user_id,)).fetchone()
    if not agent:
        conn.close()
        return jsonify([])
        
    records = conn.execute('SELECT * FROM agent_learning_records WHERE agent_id = ? ORDER BY timestamp DESC', (agent['id'],)).fetchall()
    conn.close()
    return jsonify([dict(r) for r in records])

@app.route('/api/agents/<int:user_id>/context', methods=['POST'])
def agent_add_context(user_id):
    data = request.json
    desc = data.get('description')
    context = data.get('context')
    
    conn = get_db_connection()
    agent = conn.execute('SELECT id FROM ai_agents WHERE user_id = ?', (user_id,)).fetchone()
    if not agent:
        conn.close()
        return jsonify({'error': 'Agent not found'}), 404
        
    now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    conn.execute('INSERT INTO agent_task_context (agent_id, task_description, context_data, timestamp) VALUES (?, ?, ?, ?)',
                 (agent['id'], desc, context, now))
    conn.commit()
    conn.close()
    return jsonify({'status': 'success'})

@app.route('/api/agents/<int:user_id>/context_history', methods=['GET'])
def get_agent_context_history(user_id):
    conn = get_db_connection()
    agent = conn.execute('SELECT id FROM ai_agents WHERE user_id = ?', (user_id,)).fetchone()
    if not agent:
        conn.close()
        return jsonify([])
        
    records = conn.execute('SELECT * FROM agent_task_context WHERE agent_id = ? ORDER BY timestamp DESC', (agent['id'],)).fetchall()
    conn.close()
    return jsonify([dict(r) for r in records])

def check_and_migrate_db():
    conn = get_db_connection()
    try:
        # Check if doc_id column exists in tasks
        cursor = conn.execute("PRAGMA table_info(tasks)")
        columns = [row['name'] for row in cursor.fetchall()]
        if 'doc_id' not in columns:
            print("Migrating database: Adding doc_id to tasks table...")
            conn.execute("ALTER TABLE tasks ADD COLUMN doc_id TEXT")
            conn.commit()
        if 'org_id' not in columns:
            print("Migrating database: Adding org_id to tasks table...")
            conn.execute("ALTER TABLE tasks ADD COLUMN org_id INTEGER")
            conn.commit()
            
        # Check users table
        cursor = conn.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='users'")
        if not cursor.fetchone():
            print("Migrating database: Creating users table...")
            conn.execute('''CREATE TABLE users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT UNIQUE NOT NULL,
                password_hash TEXT NOT NULL,
                role TEXT DEFAULT 'user',
                created_at TEXT
            )''')
            conn.commit()
            
        # Migrate users table to have 'type'
        cursor = conn.execute("PRAGMA table_info(users)")
        columns = [row['name'] for row in cursor.fetchall()]
        if 'type' not in columns:
            print("Migrating database: Adding type to users table...")
            conn.execute("ALTER TABLE users ADD COLUMN type TEXT DEFAULT 'human'")
            conn.commit()

        # Check organizations table
        cursor = conn.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='organizations'")
        if not cursor.fetchone():
            print("Migrating database: Creating organizations tables...")
            conn.execute('''CREATE TABLE organizations (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                parent_id INTEGER,
                manager_id INTEGER,
                created_at TEXT,
                FOREIGN KEY(parent_id) REFERENCES organizations(id),
                FOREIGN KEY(manager_id) REFERENCES users(id)
            )''')
            conn.execute('''CREATE TABLE organization_members (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                org_id INTEGER NOT NULL,
                user_id INTEGER NOT NULL,
                role TEXT DEFAULT 'member',
                joined_at TEXT,
                FOREIGN KEY(org_id) REFERENCES organizations(id),
                FOREIGN KEY(user_id) REFERENCES users(id),
                UNIQUE(org_id, user_id)
            )''')
            conn.commit()
            
        # Check AI Agents tables
        cursor = conn.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='ai_agents'")
        if not cursor.fetchone():
            print("Migrating database: Creating AI Agents tables...")
            conn.execute('''CREATE TABLE ai_agents (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER UNIQUE,
                target TEXT,
                learning_materials_path TEXT,
                operable_files TEXT,
                important_knowledge TEXT,
                FOREIGN KEY(user_id) REFERENCES users(id)
            )''')
            conn.execute('''CREATE TABLE agent_learning_records (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                agent_id INTEGER,
                content TEXT,
                source TEXT,
                timestamp TEXT,
                FOREIGN KEY(agent_id) REFERENCES ai_agents(id)
            )''')
            conn.execute('''CREATE TABLE agent_task_context (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                agent_id INTEGER,
                task_description TEXT,
                context_data TEXT,
                timestamp TEXT,
                FOREIGN KEY(agent_id) REFERENCES ai_agents(id)
            )''')
            conn.commit()

    except Exception as e:
        print(f"Migration error: {e}")
    finally:
        conn.close()

if __name__ == '__main__':
    check_and_migrate_db()
    app.run(debug=True, port=5000)
