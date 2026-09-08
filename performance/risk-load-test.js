import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 20 },
        { duration: '1m', target: 20 },
        { duration: '10s', target: 0 },
    ],
    thresholds: {
        http_req_duration: ['p(95)<200'],
    },
};

export default function () {
    const url = 'http://localhost:8080/v1/risk/evaluate';
    const payload = JSON.stringify({
        userId: 'usr_k6_test',
        amount: 100.00,
        currency: 'EUR',
        paymentMethod: 'BNPL',
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(url, payload, params);

    check(res, {
        'status is 200': (r) => r.status === 200,
        'transaction time OK': (r) => r.timings.duration < 200,
    });

    sleep(1);
}