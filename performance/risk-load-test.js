import http from 'k6/http';
import { check, sleep } from 'k6';

// 1. Configure load options and SLA thresholds
export const options = {
    stages: [
        { duration: '10s', target: 5 },  // Ramp-up to 5 virtual users (VUs)
        { duration: '20s', target: 10 }, // Steady state load: 10 VUs
        { duration: '5s', target: 0 },   // Ramp-down to 0 VUs
    ],
    thresholds: {
        // SLA: 95% of requests must complete in less than 500ms
        http_req_duration: ['p(95)<500'],
        // Allowed error rate: less than 1%
        http_req_failed: ['rate<0.01'],
    },
};

export default function () {
    // Target base URL fallback for local development
    const baseUrl = __ENV.TARGET_URL || 'http://localhost:5000';
    const url = `${baseUrl}/v1/credit-rating/usr_perf_test`;

    const payload = JSON.stringify({
        customerId: 'usr_perf_test',
        amount: 150.00,
        currency: 'EUR'
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(url, payload, params);

    // 2. Response assertions
    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1);
}