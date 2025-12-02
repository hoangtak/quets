EXTERNAL get_quest_state(quest_id)

"Xin chào, ta là nhà hiền giả D."

{ get_quest_state("CD"):
    - 1:
        #QUEST_PROGRESS:CD:2
        #SHOW_UI:Quay lại gặp NPC C
        "Ah, C sai ngươi đến à?"
        "Ngươi nói 'Hàng hóa đã sẵn sàng' phải không?"
        + [Đúng vậy.]
            "Hmm... Ta hiểu rồi."
            "Nhưng giá của C đưa ra hơi cao."
            "Hãy quay lại nói với C rằng: 'Giá cả quá cao, cần giảm 20%'."
            -> END
        + [Tôi quên mất.]
            "Ủa? Vậy ngươi quay lại hỏi C đi."
            -> END
    
    - 2:
        "Ngươi đã nói với C về giá cả chưa?"
        "Hãy quay lại C và nói: 'Giá cả quá cao, cần giảm 20%'."
        -> END
    
    - 3:
        #QUEST_COMPLETE:CD
        #REWARD:gold:150
        #REWARD:potion:2
        "Ah! C đã đồng ý giảm giá rồi à?"
        + [Đúng vậy, C đã đồng ý.]
            "Tuyệt vời! Giao dịch thành công!"
            "Đây là phần thưởng cho ngươi vì đã giúp đỡ."
            "Ngươi nhận được 150 vàng và 2 bình thuốc!"
            -> END
        + [C chưa đồng ý.]
            "Vậy à? Hãy quay lại thuyết phục C đi."
            -> END
    
    - 4:
        "Cảm ơn ngươi đã giúp đỡ giao dịch!"
        "Lần sau có việc gì, ta sẽ nhờ ngươi."
        -> END
    
    - else:
        "Ta đang nghiên cứu ma pháp."
        "Có việc gì không?"
        + [Không, tôi chỉ ghé qua.]
            "Được, hãy cẩn thận nhé."
            -> END
        + [Ông bán thuốc không?]
            "Có, nhưng hiện tại ta đang bận."
            -> END
}
