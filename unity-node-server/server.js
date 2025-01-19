const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const mysql = require('mysql2/promise');
const https = require('https');
const fs = require('fs'); // HTTPS 관련 파일 처리

const app = express();
const PORT = 3000;

// Secret key for JWT
const SECRET_KEY = 'your_secret_key';

// MySQL connection
const db = mysql.createPool({
    host: 'localhost',
    user: 'root', // MySQL 사용자 이름
    password: '1234', // MySQL 비밀번호
    database: 'my_database' // 사용할 데이터베이스 이름
});

// SSL 인증서 파일 경로 설정
const options = {
    key: fs.readFileSync('C:/Users/hhs/private-key.pem'), // private-key.pem 파일 경로
    cert: fs.readFileSync('C:/Users/hhs/certificate.pem') // certificate.pem 파일 경로
};

// Middleware
app.use(bodyParser.json());
app.use(cors());

app.get('/', (req, res) => {
    res.send('Hello HTTPS');
});

// Login
app.post('/api/login', async (req, res) => {
    const { id, password } = req.body;

    try {
        const { id, password } = req.body;
        console.log('Request Body:', req.body);

        // Check if user exists
        const [rows] = await db.query('SELECT * FROM users WHERE id = ?', [id]);
        if (rows.length === 0) {
            console.log('Invalid username');
            return res.status(401).json({ message: 'Invalid username or password' });
        }

        const user = rows[0];

        const isPasswordValid = password.trim()===user.password.trim();
        console.log(isPasswordValid);
        if (!isPasswordValid) {
            console.log('Invalid password');
            return res.status(401).json({ message: 'Invalid username or password' });
        }

        // Generate JWT
        const token = jwt.sign({ id: user.id, username: user.username }, SECRET_KEY, {
            expiresIn: '1h',
        });

        res.json({ message: 'Login successful', token });
    } catch (err) {
        console.error('Database error:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// Join
app.post('/api/register', async (req, res) => {
    const { username, id, password, email } = req.body;

    try {
        // 사용자 존재 여부 확인
        const [rows] = await db.query('SELECT * FROM users WHERE id = ?', [id]);
        if (rows.length > 0) {
            return res.status(400).json({ message: 'Username already exists' });
        }

        // 사용자 정보 DB에 저장
        await db.query('INSERT INTO users (username,id, password,email) VALUES (?, ?, ?, ?)', [username, id, password, email]);

        // 새로 가입된 사용자에 대해 JWT 생성
        const token = jwt.sign({ id, username: id }, SECRET_KEY, {
            expiresIn: '1h',
        });

        res.status(201).json({ message: 'Registration successful', token });
    } catch (err) {
        console.error('Database error:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 게시판 데이터를 페이지별로 가져오는 API
app.get('/api/board', async (req, res) => {
    const page = parseInt(req.query.page) || 1;
    const limit = 4;
    const offset = (page - 1) * limit;

    const sql = 'SELECT board.*, users.id FROM board JOIN users ON board.u_idx = users.idx ORDER BY board.b_num DESC LIMIT ?, ?';
    try {
        const [results] = await db.query(sql, [offset, limit]);
        res.json(results);  // 쿼리 결과 응답
    } catch (err) {
        console.error('Error fetching board data:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 게시물 총 갯수 얻기 (페이징을 위한 총 페이지 수 계산)
app.get('/api/board/count', async (req, res) => {
    const sql = 'SELECT COUNT(*) AS count FROM board';
    try {
        const [results] = await db.query(sql);
        res.json(results[0]);  // 결과의 첫 번째 항목만 반환
    } catch (err) {
        console.error('Error fetching board count:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 게시판 데이터 하나씩 불러오기
app.get('/api/board/info', async (req, res) => {
    const page = parseInt(req.query.page) || 1;

    const sql = 'SELECT board.*, users.id FROM board JOIN users ON board.u_idx = users.idx where b_num = ?';
    try {
        const [results] = await db.query(sql, [page]);
        res.json(results[0]);  // 쿼리 결과 응답
    } catch (err) {
        console.error('Error fetching board data:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 게시판 작성
app.post('/api/board/write', async (req, res) => {
    const { id, title,content } = req.body;

    try {
        // 사용자 아이디 번호로 바꾸기
        const [results] = await db.query('SELECT idx FROM users WHERE id = ?', [id]);
        if (results.length === 0) {
            return res.status(404).json({ message: 'User not found' }); // 사용자 존재하지 않음
        }
        const idx = results[0].idx;

        // 게시판 정보 DB에 저장
        await db.query('insert into board (u_idx,b_title,b_content,b_date) values (?,?,?,date_format(now(), \'%Y-%m-%d\'))', [idx, title,content]);

        res.status(201).json({ message: 'Board post created successfully' });
    } catch (err) {
        console.error('Database error:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 댓글 데이터를 페이지별로 가져오는 API
app.get('/api/board/comment', async (req, res) => {
    const page = parseInt(req.query.page) || 1;
    const b_num = parseInt(req.query.b_num) || 1;
    const limit = 3;
    const offset = (page - 1) * limit;

    const sql = 'SELECT commnet.*, users.id FROM commnet JOIN users ON commnet.u_idx = users.idx where commnet.b_num = ? ORDER BY commnet.c_num DESC LIMIT ?, ?';
    try {
        const [results] = await db.query(sql, [b_num, offset, limit]);
        res.json(results);  // 쿼리 결과 응답
    } catch (err) {
        console.error('Error fetching board data:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 댓글 총 갯수 얻기 (페이징을 위한 총 페이지 수 계산)
app.get('/api/board/comment/count', async (req, res) => {
    const b_num = parseInt(req.query.b_num) || 1;
    const sql = 'SELECT COUNT(*) AS count FROM commnet where b_num = ?';
    try {
        const [results] = await db.query(sql, [b_num]); // b_num을 SQL에 바인딩
        res.json(results[0]);  // 결과의 첫 번째 항목만 반환
    } catch (err) {
        console.error('Error fetching board count:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 댓글 작성
app.post('/api/board/comment/write', async (req, res) => {
    const { id, b_num,content } = req.body;

    try {
        // 사용자 아이디 번호로 바꾸기
        const [results] = await db.query('SELECT idx FROM users WHERE id = ?', [id]);
        if (results.length === 0) {
            return res.status(404).json({ message: 'User not found' }); // 사용자 존재하지 않음
        }
        const idx = results[0].idx;

        // 게시판 정보 DB에 저장
        await db.query('insert into commnet (u_idx,b_num,c_content,b_date) values (?,?,?,date_format(now(), \'%Y-%m-%d\'))', [idx, b_num,content]);

        res.status(201).json({ message: 'Comment created successfully' });
    } catch (err) {
        console.error('Database error:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 게시글 삭제 API
app.delete('/api/board/delete', (req, res) => {
    const { b_num } = req.body; // 요청 본문에서 b_num을 받음

    // MySQL 쿼리 (게시글 삭제)
    const query = 'DELETE FROM board WHERE b_num = ?';

    db.query(query, [b_num], (err, results) => {
        if (err) {
            console.error('Error deleting post:', err);
            return res.status(500).json({ message: 'Internal server error' });
        }

        // 결과 처리 (삭제 성공)
        if (results.affectedRows > 0) {
            return res.status(200).json({ message: 'Post deleted successfully' });
        } else {
            return res.status(404).json({ message: 'Post not found' });
        }
    });
});

// 레이싱 기록 작성
app.post('/api/raceScore', async (req, res) => {
    const { id, r_map,r_time } = req.body;

    try {
        // 사용자 아이디 번호로 바꾸기
        const [results] = await db.query('SELECT idx FROM users WHERE id = ?', [id]);
        if (results.length === 0) {
            return res.status(404).json({ message: 'User not found' }); // 사용자 존재하지 않음
        }
        const idx = results[0].idx;

        // 게시판 정보 DB에 저장
        await db.query('INSERT INTO race_records (u_idx, r_map, r_time,r_date) VALUES (?, ?, ?, CURRENT_TIMESTAMP)', [idx, r_map, r_time]);

        res.status(201).json({ message: 'raceScore successfully' });
    } catch (err) {
        console.error('Database error:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 레이싱 최고기록 불러오기
app.get('/api/raceScore/mybest', async (req, res) => {
    const { id, r_map } = req.query; // 쿼리 파라미터에서 u_idx와 r_map 가져오기

    try {
        // 사용자 아이디 번호로 바꾸기
        const [userResults] = await db.query('SELECT idx FROM users WHERE id = ?', [id]);
        if (userResults.length === 0) {
            return res.status(404).json({ message: 'User not found' }); // 사용자 존재하지 않음
        }
        const u_idx = userResults[0].idx;

        const sql = 'SELECT r_time FROM race_records where u_idx = ? and r_map = ? ORDER BY r_time asc';

        const [results] = await db.query(sql, [u_idx, r_map]);
        // 결과가 없을 경우 처리
        if (results.length === 0) {
            res.json({ bestTime: '00:00' });
        }

        // 최고 기록 반환
        res.json({ bestTime: results[0].r_time });
    } catch (err) {
        console.error('Error fetching race record:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// 레이싱 순위
app.get('/api/raceScore/bestscore', async (req, res) => {

    const sql = 'SELECT users.id, race_records.r_map, race_records.r_time FROM race_records JOIN users ON race_records.u_idx = users.idx ORDER BY r_time asc limit 3';
    try {
        const [results] = await db.query(sql);
        res.json(results);  // 쿼리 결과 응답
    } catch (err) {
        console.error('Error fetching board data:', err);
        res.status(500).json({ message: 'Internal server error' });
    }
});


// Protected endpoint
app.get('/api/protected', (req, res) => {
    const token = req.headers.authorization?.split(' ')[1];
    if (!token) {
        return res.status(401).json({ message: 'No token provided' });
    }

    try {
        const decoded = jwt.verify(token, SECRET_KEY);
        res.json({ message: 'Welcome to the protected route', user: decoded });
    } catch (err) {
        res.status(401).json({ message: 'Invalid or expired token' });
    }
});

// HTTPS 서버 시작
https.createServer(options, app).listen(PORT, () => {
    console.log(`Server running on https://localhost:${PORT}`);
});