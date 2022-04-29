## issue: SonarCloud #1

A laboron tanultak alapján a repository-t integráltam a SonarCloud rendszerbe.

A megvalósítás során problémába ütköztem a projekt inicializálásakor, ugyanis, bár admin jogom van a repo-n, az organizationnak nem vagyok ownere. 

- Teamsen kértem segítséget, és gyorsan kaptam is.

Létrehoztam az összekötést a repo és a SonarCloud között (secret), majd elkészítettem a sonar.yml file-ban a leírót a github action-höz.