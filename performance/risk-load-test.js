import http from 'k6/http';
import { check, fail, sleep } from 'k6';

// Environment configuration with sensible defaults
const BASE_URL = __ENV.BASE_URL || 'http://localhost:8080';

export const options = {
    stages: [
        { duration: '30s', target: 20 }, // Ramp-up
        { duration: '1m', target: 20 },  // Sustained load
        { duration: '10s', target: 0 },   // Ramp-down
    ],
    thresholds: {
        // Core performance SLAs
        http_req_duration: ['p(95)<200'], // 95% of requests must complete under 200ms
        http_req_failed: ['rate<0.01'],    // Error rate must be less than 1%
        checks: ['rate>0.99'],            // At least 99% of checks must pass
    },
};

export default function () {
    const url = `${BASE_URL}/v1/risk/evaluate`;

    const payload = JSON.stringify({
        userId: `usr_k6_${__VU}_${__ITER}`, // Dynamic ID per Virtual User / Iteration
        amount: 100.00,
        currency: 'EUR',
        paymentMethod: 'BNPL',
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
        },
        tags: { name: 'RiskEvaluation' },
    };

    const res = http.post(url, payload, params);

    // Validate HTTP Status & Payload Integrity
    const isSuccess = check(res, {
        'status is 200 or 201': (r) => r.status === 200 || r.status === 201,
        'response time p(95) OK': (r) => r.timings.duration < 200,
        'has valid JSON body': (r) => {
            try {
                return r.json() !== null;
            } catch (e) {
                return false;
            }
        },
    });

    if (!isSuccess) {
        // Surface details for failing requests in debug output
        console.error(`[LoadTestError] VU:${__VU} Status:${res.status} Body:${res.body}`);
    }

    // Pace requests to prevent artificially high throughput loops
    sleep(1);
}