EXTERNAL get_quest_state(quest_id)

"Chào mừng ngươi đến tiệm thuốc của ta."

{ get_quest_state("HERB"):
    - 0:
        "Ta là thầy thuốc của làng này."
        "Gần đây, ta đang thiếu nguyên liệu để pha chế thuốc."
        + [Tôi có thể giúp gì không?]
            -> offer_quest
        + [Tôi chỉ đi ngang qua thôi.]
            "Ồ, vậy à. Hãy cẩn thận nhé."
            -> END
    
    - 1:
        #CHECK_QUEST:HERB
        { get_quest_state("HERB"):
            - 2:
                -> complete_quest
            - else:
                -> state_1_dialogue
        }
    
    - 2:
        -> complete_quest
    
    - 5:
        "Cảm ơn ngươi đã giúp đỡ!"
        "Nhờ ngươi mà dân làng có thuốc chữa bệnh."
        + [Ông cần thêm nguyên liệu không?]
            -> check_new_quest
        + [Vui lòng được giúp.]
            "Ngươi thật tốt bụng. Nếu cần thuốc, cứ đến gặp ta."
            -> END
    
    - else:
        "Có chuyện gì không?"
        -> END
}

=== offer_quest ===
"Thật sao? Tuyệt vời!"
"Ta đang cần Thảo Dược Đỏ để pha chế thuốc chữa bệnh."
"Loại thảo dược này rất hiếm, chúng chỉ mọc ở khu rừng sâu."
+ [Tôi cần thu thập bao nhiêu?]
    #COLLECT_QUEST:HERB:Thảo Dược Đỏ:5
    "Ta cần 5 cây Thảo Dược Đỏ."
    "Chúng thường mọc gần các tảng đá ở khu rừng phía đông."
    "Hãy cẩn thận, khu đó có nhiều quái vật!"
    -> END
+ [Nghe có vẻ nguy hiểm quá.]
    "Ồ... vậy à."
    "Nếu ngươi đổi ý, hãy quay lại gặp ta nhé."
    -> END

=== state_1_dialogue ===
"Ngươi đã thu thập được bao nhiêu thảo dược rồi?"
"Ta cần 5 cây Thảo Dược Đỏ để pha chế."
+ [Cho tôi xem lại nhiệm vụ.]
    "Được, hãy đi thu thập 5 cây Thảo Dược Đỏ."
    "Chúng thường mọc gần các tảng đá ở khu rừng phía đông."
    -> END
+ [Tôi sẽ đi tìm ngay.]
    "Tốt lắm! Hãy cẩn thận nhé."
    -> END

=== complete_quest ===
#REMOVE_ITEMS:Thảo Dược Đỏ:5
#QUEST_COMPLETE:HERB
#REWARD:gold:100
#REWARD:potion:3
"Ah! Ngươi đã thu thập đủ rồi!"
"Để ta xem... Ồ, chất lượng tuyệt vời!"
"Với những nguyên liệu này, ta có thể cứu được nhiều người."
"Đây là phần thưởng xứng đáng cho ngươi!"
"Ngươi nhận được 100 vàng và 3 bình thuốc hồi máu!"
-> END

=== check_new_quest ===
"Hmm... thực ra ta còn cần thêm vài thứ nữa."
"Nhưng chúng còn nguy hiểm hơn Thảo Dược Đỏ."
+ [Tôi sẵn sàng!]
    "Thật dũng cảm! Nhưng hãy quay lại sau."
    "Ta cần thời gian chuẩn bị danh sách."
    -> END
+ [Để lần sau vậy.]
    "Được, hãy nghỉ ngơi đi. Lần sau ta sẽ giao cho ngươi nhiệm vụ khó hơn."
    -> END