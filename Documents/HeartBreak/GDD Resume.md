
## Pitch

Heartbreak est un jeu de bossrush, de combat en temps réel.
Inspiré par l'esthétique de Sayonara Wild Hearts, C'est un jeu coloré, stylisé.

Heartbreak explore la thématique du syndrome du sauveur, et de blesser les gens qui nous aime car on ne peux pas accepter qu'on est capable d'être aimée.

Inspiré par les combat de samouraï, non dans le style de combat, mais dans le conflit d'objectif. Duel d'honneur, ou on respecte la personne qu'on provoque en duel.

Amae, la protagoniste, se retrouve à gravir la **Tour d'Ivoire**. Étage par étage. Combat par combat, pour sauver Itami, un femme enfermée toute en haut de la tour à cause de ses pouvoir destructeur qu'elle ne controle pas.

L'objectif est de vaincre le boss, en réduisant sa barre de vie à zéro.

## L'arène

L'arène est circulaire, composée d'anneau concentriques.
Le bosse reste toujours au centre de l'arène.
La progression est d'avancer d'anneau en anneau, mais qui rendre l'esquive plus dure.
## 3C

Le personnage jouable peux se déplacer librement sur l'anneau où elle se situe.
La caméra est situé derieère le joueur, avec un focus / target sur le boss.
### Controls et Actions

Déplacement, Horaire / Anti-Horaire. Fluide et continue.
Pour avancer d'un anneau, les joueur.euses doivent maintenir Forward, selon le temps dépensé, le nombre d'anneau qui seront dashés, l'action fera forcement attérire dans un anneau entier. pas de float.

Si jamais l'anneau visé est au dela de l'anneau le plus proche. L'excèdent est converti en attaque, scalé par le nombre d'overflow. (peut être des attaque different selon le tier d'overflow)

Le joueur peut aussi hop en arrière instantanement, et dasher orbitalement, avec une recharge timée. 
Le joueur peur sauter, mais aussi dash orbital en l'air.

## Boss

Le boss à des attaque variées, pour compenser son immoblité :
- Attaque en cone
- Attaque radiale
- attaque radiale type onde de shock.
- Poser des minions, qui s'active soit par timing, soit par detection joueur.

La vie du boss est particuliaire : Divisé en segement. Quand un segment est vide, un point fiable directionnel apparait, et il faut le briser avec une attaque pour déverouiller le prochain segment.

## Boucle

Le joueur doit avancer, reculer, se prossitionner pour avoir une opportunité d'attaque.