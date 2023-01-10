extern crate actix_web;
#[macro_use]
extern crate json;
extern crate reqwest;

use actix_web::{web, App, HttpRequest, HttpResponse, HttpServer};
use json::{parse, JsonValue};
use std::iter::Iterator;

async fn reddit() -> JsonValue {
    let url = "https://www.reddit.com/r/politics/hot.json";
    let body = reqwest::get(url)
        .await
        .expect("Could not query reddit")
        .text()
        .await
        .expect("Could not get body from querying reddit");
    parse(&body[..]).expect("Could not parse reddit response")
}

fn children(val: &JsonValue) -> impl Iterator<Item = &JsonValue> {
    let children = &val["data"]["children"];
    children.members().map(|l| &l["data"])
}

async fn index(_req: HttpRequest) -> HttpResponse {
    let resp = reddit().await;
    let mut arr = JsonValue::new_array();
    for listing in children(&resp) {
        let row = object! {
            "title" => listing["title"].clone()
        };
        arr.push(row).unwrap();
    }
    HttpResponse::Ok()
        .content_type("text/json")
        .body(arr.pretty(2))
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    HttpServer::new(|| App::new().route("/", web::get().to(index)))
        .bind(("127.0.0.1", 8080))?
        .run()
        .await
}
