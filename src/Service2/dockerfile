from microsoft/aspnet
ADD . /app
WORKDIR /app
RUN dnu restore
EXPOSE 5000/tcp
ENTRYPOINT ["dnx", "web", "--server.urls", "http://0.0.0.0:5000"]