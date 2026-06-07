#!/bin/bash
while true; do
  echo -e "HTTP/1.1 200 OK\nContent-Type: application/json\n\n{\"status\":\"confirmed\",\"appointment_id\":123}" | nc -l -p 3000 -q 1
done
