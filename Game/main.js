$(function () {
    //�Ƿ������Ԫ��
    var isNewRndItem = false;
    var gameScore = 0;
    //��߷�
    var maxScore = 0;
	//����λ��
	var currentPosition={
		pos : 0,
		value:0
	}
    if (localStorage.maxScore) {
        maxScore = localStorage.maxScore - 0;
    } else {
        maxScore = 0;
    }

    //��Ϸ��ʼ��
    gameInit();

    function refreshGame() {
        var items = $('.gameBody .row .item');
        for (var i = 0; i < items.length; i++) {
            items.eq(i).html('').removeClass('nonEmptyItem').addClass('emptyItem');
        }
        gameScore = 0;
        //��������
        $('#gameScore').html(gameScore);
        //�������������Ԫ��
        newRndItem();
        newRndItem();
        //ˢ����ɫ
        refreshColor();
        $('#gameOverModal').modal('hide');
    }


    function getSideItem(currentItem, direction) {
        //��ǰԪ�ص�λ��
        var currentItemX = currentItem.attr('x') - 0;
        var currentItemY = currentItem.attr('y') - 0;

        //���ݷ����ȡ�Ա�Ԫ�ص�λ��
        switch (direction) {
            case 'left':
                var sideItemX = currentItemX;
                var sideItemY = currentItemY - 1;
                break;
            case 'right':
                var sideItemX = currentItemX;
                var sideItemY = currentItemY + 1;
                break;
            case 'up':
                var sideItemX = currentItemX - 1;
                var sideItemY = currentItemY;
                break;
            case 'down':
                var sideItemX = currentItemX + 1;
                var sideItemY = currentItemY;
                break;
        }
        //�Ա�Ԫ��
        var sideItem = $('.gameBody .row .x' + sideItemX + 'y' + sideItemY);
        return sideItem;
    }


    function itemMove(currentItem, direction) {

        var sideItem = getSideItem(currentItem, direction);

        if (sideItem.length == 0) {//��ǰԪ���������
            //����

        } else if (sideItem.html() == '') { //��ǰԪ�ز������һ�������ҡ��ϡ��£���Ԫ���ǿ�Ԫ��
            sideItem.html(currentItem.html()).removeClass('emptyItem').addClass('nonEmptyItem');
            currentItem.html('').removeClass('nonEmptyItem').addClass('emptyItem');
            itemMove(sideItem, direction);
            isNewRndItem = true;

        } else if (sideItem.html() != currentItem.html()) {//���ҡ��ϡ��£���Ԫ�غ͵�ǰԪ�����ݲ�ͬ
            //����

        } else {//���ҡ��ϡ��£���Ԫ�غ͵�ǰԪ��������ͬ
            //���Һϲ�
            sideItem.html((sideItem.html() - 0) * 2);
            currentItem.html('').removeClass('nonEmptyItem').addClass('emptyItem');
            gameScore += (sideItem.text() - 0) * 10;
            $('#gameScore').html(gameScore);
            // itemMove(sideItem, direction);
            maxScore = maxScore < gameScore ? gameScore : maxScore;
            $('#maxScore').html(maxScore);
            localStorage.maxScore = maxScore;
            isNewRndItem = true;
            return;
        }
    }


    function move(direction) {
        //��ȡ���зǿ�Ԫ��
        var nonEmptyItems = $('.gameBody .row .nonEmptyItem');
        //������µķ���������ϣ�����������ǿ�Ԫ��
		var k = direction == 'left' || direction == 'up';
		var N = nonEmptyItems.length;
		var K = 2*k-1;
		var B=(1-k)*(N-1);
		for (var i = 0; i < nonEmptyItems.length; i++) {
			var index = K*i+B
			var currentItem = nonEmptyItems.eq(index);
			itemMove(currentItem, direction);
		}

        //�Ƿ������Ԫ��
        if (isNewRndItem) {
            newRndItem();
            refreshColor();
        }
    }

    function isGameOver() {
        //��ȡ����Ԫ��
        var items = $('.gameBody .row .item');
        //��ȡ���зǿ�Ԫ��
        var nonEmptyItems = $('.gameBody .row .nonEmptyItem');
        if (items.length == nonEmptyItems.length) {//����Ԫ�صĸ��� == ���зǿ�Ԫ�صĸ���  ��û�п�Ԫ��
            //�������зǿ�Ԫ��
            for (var i = 0; i < nonEmptyItems.length; i++) {
                var currentItem = nonEmptyItems.eq(i);
                if (getSideItem(currentItem, 'up').length != 0 && currentItem.html() == getSideItem(currentItem, 'up').html()) {
                    //�ϱ�Ԫ�ش��� �� ��ǰԪ���е����ݵ����ϱ�Ԫ���е�����
                    return;
                } else if (getSideItem(currentItem, 'down').length != 0 && currentItem.html() == getSideItem(currentItem, 'down').html()) {
                    //�±�Ԫ�ش��� �� ��ǰԪ���е����ݵ����±�Ԫ���е�����
                    return;
                } else if (getSideItem(currentItem, 'left').length != 0 && currentItem.html() == getSideItem(currentItem, 'left').html()) {
                    //���Ԫ�ش��� �� ��ǰԪ���е����ݵ������Ԫ���е�����
                    return;
                } else if (getSideItem(currentItem, 'right').length != 0 && currentItem.html() == getSideItem(currentItem, 'right').html()) {
                    //�ұ�Ԫ�ش��� �� ��ǰԪ���е����ݵ����ұ�Ԫ���е�����
                    return;
                }
            }
        } else {
            return;
        }
        $('#gameOverModal').modal('show');
    }

    //��Ϸ��ʼ��
    function gameInit() {
        //��ʼ������
        $('#gameScore').html(gameScore);
        //����ֵ
        $('#maxScore').html(maxScore);
        //Ϊˢ�°�ť���¼�
        $('.refreshBtn').click(refreshGame);
		$('.currentpositonResetBtn').click(updatecurrentpositon);
		
        //�������������Ԫ��
        newRndItem();
        newRndItem();
        //ˢ����ɫ
        refreshColor();
    }

    //���������Ԫ��
    function newRndItem() {
        //�������������
        var newRndArr = [2, 2, 4];
        var newRndNum = newRndArr[getRandom(0, 2)];
        //������������ֵ�λ��
        var emptyItems = $('.gameBody .row .emptyItem');
        var newRndSite = getRandom(0, emptyItems.length - 1);
        emptyItems.eq(newRndSite).html(newRndNum).removeClass('emptyItem').addClass('nonEmptyItem');
		currentPosition.pos = (emptyItems.eq(newRndSite).attr('x') - 0)* 4 + (emptyItems.eq(newRndSite).attr('y')-0);
		currentPosition.value = newRndNum;
		console.log('currentposition\r\n pos:'+currentPosition.pos +'  value:'+ currentPosition.value);
    }

    //���������������min��max
    function getRandom(min, max) {
        return min + Math.floor(Math.random() * (max - min + 1));
    }
	
	//���ڣ�ˢ�����һ��
	function updatecurrentpositon(){
		var items = $('.gameBody .item');
		items.eq(currentPosition.pos).html('').removeClass('nonEmptyItem').addClass('emptyItem');
		newRndItem();
		//ˢ����ɫ
        refreshColor();
	}

    //ˢ����ɫ
    function refreshColor() {
        var items = $('.gameBody .item');
        for (var i = 0; i < items.length; i++) {
            // console.log(items.eq(i).parent().index());
            switch (items.eq(i).html()) {
                case '':
                    items.eq(i).css('background', '');
                    break;
                case '2':
                    items.eq(i).css('background', 'rgb(250, 225, 188)');
                    break;
                case '4':
                    items.eq(i).css('background', 'rgb(202, 240, 240)');
                    break;
                case '8':
                    items.eq(i).css('background', 'rgb(117, 231, 193)');
                    break;
                case '16':
                    items.eq(i).css('background', 'rgb(240, 132, 132)');
                    break;
                case '32':
                    items.eq(i).css('background', 'rgb(181, 240, 181)');
                    break;
                case '64':
                    items.eq(i).css('background', 'rgb(182, 210, 246)');
                    break;
                case '128':
                    items.eq(i).css('background', 'rgb(255, 207, 126)');
                    break;
                case '256':
                    items.eq(i).css('background', 'rgb(250, 216, 216)');
                    break;
                case '512':
                    items.eq(i).css('background', 'rgb(124, 183, 231)');
                    break;
                case '1024':
                    items.eq(i).css('background', 'rgb(225, 219, 215)');
                    break;
                case '2048':
                    items.eq(i).css('background', 'rgb(221, 160, 221)');
                    break;
                case '4096':
                    items.eq(i).css('background', 'rgb(250, 139, 176)');
                    break;
            }
        }
    }

    // ���Եķ���������¼�
    $('body').keydown(function (e) {
        switch (e.keyCode) {
            case 37:
                // left
                console.log('left');
                isNewRndItem = false;
                move('left');
                isGameOver();
                break;
            case 38:
                // up
                console.log('up');
                isNewRndItem = false;
                move('up');
                isGameOver();
                break;
            case 39:
                // right
                console.log('right');
                isNewRndItem = false;
                move('right');
                isGameOver();
                break;
            case 40:
                // down
                console.log('down');
                isNewRndItem = false;
                move('down');
                isGameOver();
                break;
        }
    });

    // �ֻ���Ļ��������
    (function () {
        mobilwmtouch(document.getElementById("gameBody"))
        document.getElementById("gameBody").addEventListener('touright', function (e) {
            e.preventDefault();
            // alert("��������");
            console.log('right');
            isNewRndItem = false;
            move('right');
            isGameOver();
        });
        document.getElementById("gameBody").addEventListener('touleft', function (e) {
            // alert("��������");
            console.log('left');
            isNewRndItem = false;
            move('left');
            isGameOver();
        });
        document.getElementById("gameBody").addEventListener('toudown', function (e) {
            // alert("��������");
            console.log('down');
            isNewRndItem = false;
            move('down');
            isGameOver();
        });
        document.getElementById("gameBody").addEventListener('touup', function (e) {
            // alert("��������");
            console.log('up');
            isNewRndItem = false;
            move('up');
            isGameOver();
        });

        function mobilwmtouch(obj) {
            var stoux, stouy;
            var etoux, etouy;
            var xdire, ydire;
            obj.addEventListener("touchstart", function (e) {
                stoux = e.targetTouches[0].clientX;
                stouy = e.targetTouches[0].clientY;
                //console.log(stoux);
            }, false);
            obj.addEventListener("touchend", function (e) {
                etoux = e.changedTouches[0].clientX;
                etouy = e.changedTouches[0].clientY;
                xdire = etoux - stoux;
                ydire = etouy - stouy;
                chazhi = Math.abs(xdire) - Math.abs(ydire);
                //console.log(ydire);
                if (xdire > 0 && chazhi > 0) {
                    console.log("right");
                    //alert(evenzc('touright',alerts));
                    obj.dispatchEvent(evenzc('touright'));

                } else if (ydire > 0 && chazhi < 0) {
                    console.log("down");
                    obj.dispatchEvent(evenzc('toudown'));
                } else if (xdire < 0 && chazhi > 0) {
                    console.log("left");
                    obj.dispatchEvent(evenzc('touleft'));
                } else if (ydire < 0 && chazhi < 0) {
                    console.log("up");
                    obj.dispatchEvent(evenzc('touup'));
                }
            }, false);

            function evenzc(eve) {
                if (typeof document.CustomEvent === 'function') {

                    this.event = new document.CustomEvent(eve, {//�Զ����¼�����
                        bubbles: false,//�Ƿ�ð��
                        cancelable: false//�Ƿ����ֹͣ����
                    });
                    if (!document["evetself" + eve]) {
                        document["evetself" + eve] = this.event;
                    }
                } else if (typeof document.createEvent === 'function') {


                    this.event = document.createEvent('HTMLEvents');
                    this.event.initEvent(eve, false, false);
                    if (!document["evetself" + eve]) {
                        document["evetself" + eve] = this.event;
                    }
                } else {
                    return false;
                }

                return document["evetself" + eve];

            }
        }
    })();
});