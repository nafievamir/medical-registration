#!/bin/bash
while true; do
  echo -e "HTTP/1.1 200 OK\nContent-Type: application/json\n\n{\"slots\":[\"10:00\",\"10:30\",\"11:00\"]}" | nc -l -p 5002 -q 1
done
