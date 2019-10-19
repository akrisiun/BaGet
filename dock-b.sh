#
# docker build

dotnet --info | head -n 20

docker rmi -f baget
docker build . -t baget

echo "for run :5005"
echo "docker run -d -p 5005:5000 --name baget5005 baget"

