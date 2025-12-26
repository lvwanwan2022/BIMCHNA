import os
import shutil
import sqlite3
import sys

# Windows console encoding fix
if sys.platform == 'win32':
    try:
        sys.stdout.reconfigure(encoding='utf-8')
    except:
        pass

DB_NAME = 'doc_system.db'
TARGET_DIR = 'docs/Level_1'

def move_and_update():
    if not os.path.exists(TARGET_DIR):
        os.makedirs(TARGET_DIR)
        print(f"Created directory {TARGET_DIR}")

    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    
    # robust approach: iterate files in current dir
    for filename in os.listdir('.'):
        if not filename.endswith('.md'):
            continue
            
        # Check if it looks like a level 1 file (id_1...)
        # Pattern: ..._id_1xxxxxxx_...
        # But be careful not to move things we shouldn't.
        # Let's try to extract ID and check against DB or just check the ID string.
        
        parts = filename.split('_')
        doc_id = None
        for i, part in enumerate(parts):
            if part == 'id' and i+1 < len(parts):
                doc_id = parts[i+1]
                break
        
        if doc_id and doc_id.startswith('1'): # Level 1 IDs start with 1
             # Move it
             new_path = os.path.join(TARGET_DIR, filename)
             try:
                 shutil.move(filename, new_path)
                 print(f"Moved {filename} -> {new_path}")
                 
                 # Update DB
                 # We need to match the DB record. The filename in DB might be slightly different encoding-wise
                 # So we update by ID.
                 c.execute("UPDATE documents SET path = ?, filename = ? WHERE id = ?", (TARGET_DIR + '/', filename, doc_id))
                 print(f"Updated DB for ID {doc_id}")
             except Exception as e:
                 print(f"Error moving {filename}: {e}")

    conn.commit()
    conn.close()

if __name__ == '__main__':
    move_and_update()

