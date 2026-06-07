#!/bin/bash
while true; do
  echo -e "HTTP/1.1 200 OK\nContent-Type: application/json\n\n[{\"id\":1,\"name\":\"Иванов И.И.\",\"specialty\":\"Терапевт\"},{\"id\":2,\"name\":\"Петрова А.С.\",\"specialty\":\"Кардиолог\"}]" | nc -l -p 5001 -q 1
done
