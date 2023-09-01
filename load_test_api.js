import http from 'k6/http';
import { check, sleep } from 'k6';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

export const options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        // A list of virtual users { target: ..., duration: ... } objects that specify 
        // the target number of VUs to ramp up or down to for a specific period.
        { duration: '1m', target: 10 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
        { duration: '1m', target: 10 }, // stay at 100 users for 10 minutes
        { duration: '1m', target: 0 }, // ramp-down to 0 users
    ],
    thresholds: {
        // A collection of threshold specifications to configure under what condition(s) 
        // a test is considered successful or not
        'http_req_duration': ['p(99)<1500'], // 99% of requests must complete below 1.5s
        //'logged in successfully': ['p(99)<1500'], // 99% of requests must complete below 1.5s
    }
};

export default function () {

	let my_id = getRandonId();

	const payload = JSON.stringify({
	    name: 'user' + my_id,
		username: 'user' + my_id,
		email: 'aaa@aaa.com',
		password: 'user' + my_id,
		role: [ 0, 1, 2 ]
	  });

	  const params = {
		headers: {
		  'Content-Type': 'application/json',
		},
	  };

    // Here, we set the endpoint to test.
    const response = http.post('http://localhost:5000/api/users', payload, params);

    // An assertion
    check(response, {
        'is status 200': (x) => x.status === 200
    });

    sleep(1);
}

function getRandonId(){
    let randIndex =  getRandomInt(0, 100);
    return randIndex;
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

export function handleSummary(data) {
  return {
    "summary.html": htmlReport(data),
  };
}
