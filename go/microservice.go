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
		req.Header.Set("user-agent", "microservice-comparison-go")
		req.Header.Set("accept", "application/json")
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
