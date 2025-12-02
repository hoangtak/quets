EXTERNAL get_quest_state(quest_id)

"Chào ngươi, ta là thương nhân C."

{ get_quest_state("CD"):
    - 0:
        "Ta có một giao dịch quan trọng với nhà hiền giả D."
        "Nhưng ta đang bận, ngươi có thể giúp ta không?"
        + [Được, tôi có thể giúp gì?]
            #QUEST_START:CD
            #SHOW_UI:Đi gặp NPC D
            "Tuyệt! Hãy đến gặp D ở phía nam."
            "Nói với người ấy rằng: 'Hàng hóa đã sẵn sàng'."
            -> END
        + [Xin lỗi, tôi bận.]
            "Không sao, hãy quay lại sau."
            -> END
    
    - 1:
        "Ngươi đã đến gặp D chưa?"
        "Hãy nhanh lên, giao dịch này rất quan trọng!"
        -> END
    
    - 2:
        #QUEST_PROGRESS:CD:3
        #SHOW_UI:Quay lại gặp NPC D
        "Ah! Ngươi đã quay về!"
        "Vậy D nói gì?"
        + [D bảo: 'Giá cả quá cao, cần giảm 20%']
            "Cái gì?! Giảm 20%?!"
            "Hừm... được thôi. Ta đồng ý."
            "Hãy quay lại gặp D và nói: 'Đã đồng ý giảm giá'."
            -> END
        + [D không nói gì cả.]
            "Lạ nhỉ... Hãy quay lại hỏi D xem sao."
            -> END
    
    - 3:
        "Ngươi đã nói với D về việc giảm giá chưa?"
        "Hãy nhanh lên!"
        -> END
    
    - 4:
        "Tuyệt vời! Giao dịch đã hoàn tất!"
        "Cảm ơn ngươi đã giúp đỡ."
        + [Vui lòng được giúp.]
            "Ngươi thật tốt bụng."
            -> END
        + [Tôi có được gì không?]
            "Ha ha, dĩ nhiên rồi! Lần sau ta sẽ cho ngươi giá ưu đãi."
            -> END
    
    - else:
        "Có chuyện gì không?"
        -> END
}