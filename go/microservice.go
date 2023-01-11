package main

import (
	"encoding/json"
	"github.com/labstack/echo"
	"io/ioutil"
	"net/http"
)

//Reddit anonymous json access is really ugly in go std
type Reddit struct {
	Data struct {
		Children []struct {
			Data struct {
				Title string `json:"title"`
			} `json:"data"`
		} `json:"children"`
	} `json:"data"`
}

func main() {
	e := echo.New()
	e.GET("/", func(c echo.Context) error {
		client := http.DefaultClient
		req, err := http.NewRequest("GET", "https://www.reddit.com/r/politics/hot.json", nil)
		if err != nil {
            e.Logger.Print(err)
			return err
		}
		// reddit is blacklisting golang client (such workarounds are not working)
		req.Header.Set("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36")
		req.Header.Set("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
        req.Header.Set("accept-encoding", "gzip, deflate, br")
        req.Header.Set("accept-language", "en-US,en;q=0.9,pt-BR;q=0.8,pt;q=0.7")
        req.Header.Set("cache-control", "no-cache")
        req.Header.Set("dnt","1")
        req.Header.Set("pragma", "no-cache")
		resp, err := client.Do(req)
		if err != nil {
            e.Logger.Print(err)
			return err
		}
		body, err := ioutil.ReadAll(resp.Body)
		if err != nil {
            e.Logger.Print(err)
			return err
		}
        e.Logger.Print(string(body[:]))
		var data Reddit
		err = json.Unmarshal(body, &data)
		if err != nil {
            e.Logger.Print(err)
			return err
		}
		var arr []interface{}
		for _, listing := range data.Data.Children {
			arr = append(arr, map[string]interface{}{
				"title": listing.Data.Title,
			})
			e.Logger.Print(listing)
		}
		return c.JSON(http.StatusOK, arr)
	})
	e.Logger.Fatal(e.Start(":8080"))
}
