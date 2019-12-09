using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model {
    public class Player {
        public int hp;
        public RoleType roleType;

        public Player(int hp, RoleType roleType) {
            this.hp = hp;
            this.roleType = roleType;
        }

        public bool TakeDamage(int damage) {
            hp -= damage;
            hp = Math.Max(hp, 0);
            if (hp <= 0) return true;
            return false;
        }
        public bool IsDie() {
            return hp <= 0;
        }
    }
}
