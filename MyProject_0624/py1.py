import simplejson
import urllib

ELEVATION_BASE_URL = 'https://maps.googleapis.com/maps/api/elevation/json'
CHART_BASE_URL = 'https://chart.apis.google.com/chart'


def getElevation(path="36.578581,-118.291994|36.23998,-116.83171",samples="100", **elvtn_args):
      elvtn_args.update({
        'path': path,
        'samples': samples
      })

      url = ELEVATION_BASE_URL + '?' + urllib.urlencode(elvtn_args)
      response = simplejson.load(urllib.urlopen(url))

      # Create a dictionary for each results[] object
      elevationArray = []

      for resultset in response['results']:
        elevationArray.append(resultset['elevation'])
        print resultset['elevation']

      

if __name__ == '__main__':
    # Mt. Whitney
    startStr = "36.578581,-118.291994"
    # Death Valley
    endStr = "36.23998,-116.83171"

    pathStr = startStr + "|" + endStr

    getElevation(pathStr)
