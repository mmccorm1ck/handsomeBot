FROM node:lts-alpine3.22

LABEL org.opencontainers.image.source=https://github.com/mmccorm1ck/handsomeBot

WORKDIR /app

COPY ./server .

RUN npm install

ENV PORT=3000

EXPOSE 3000

CMD ["npx", "ts-node", "src/index.ts"]
