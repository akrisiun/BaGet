
# docker rm -f baget5005
# docker run -d -p 5005:5000 --name baget5005 baget
# docker logs baget5005

docker rm -f baget90

mkdir  ../Packages
echo $(pwd)/../Packages

docker run -d -p 90:5000 -v $(pwd)/../Packages:/app/packages --name baget90 baget

docker ps | head -n 3
docker logs baget90
docker exec baget90 ls /app/packages
