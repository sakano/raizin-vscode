# eve_start
発生条件ブロックの開始
- rai7,rai8,command,control

# eve_end
発生条件ブロックの終了
- rai7,rai8,command,control

# if_union1,覇王ID:RulerId
覇王IDが同盟中である場合にイベント発生。
同盟盟主覇王IDがpvalに設定されます。
- rai7,rai8,if
 
# if_union2,覇王ID1:RulerId,覇王ID2:RulerId,モード:Choice|0:未設定|1:通常同盟|2:従属同盟(1が2に従属)|3:従属同盟(1か2が従属)
覇王ID1と覇王ID2は同盟関係である場合にイベント発生。

モード
  0:未設定（従属状態は関係なし。if_uniと同じ動作）
  1:通常同盟である（覇王ID1、覇王ID2どちらも従属ではない）
  2:従属同盟である（覇王ID1は通常、覇王ID2は従属状態）
  3:従属同盟である（覇王ID1もしくは覇王ID2が従属状態）
- rai7,rai8,if

# if_union_meisyu,覇王ID:RulerId
覇王IDは同盟盟主である場合にイベント発生。
- rai7,if

# if_not_union,覇王ID1:RulerId,覇王2:RulerId
- rai8,if

# if_soubi_item,人物ID:PersonId,アイテム名:ItemName
指定の人物IDが指定のアイテムを装備しているか？

例）人物ID50が一族の証を装備している場合にイベント発生。
if_soubi_item,50,一族の証
- rai7,if

# if_not_soubi_item,人物ID:PersonId,アイテム名:ItemName
指定の人物IDが指定のアイテムを装備していない場合にイベント発生。

例）人物ID50が一族の証を装備している場合にイベント発生。
if_soubi_item,50,一族の証
- rai7,if

# if_mappos,X:Int,Y:Int,HEX種別:Choice|0:フリーエリア|1:小惑星|2:ブラックホール|3:恒星|4:航行不可|5:惑星|11:艦隊|16:要塞
指定座標XYのHEX種別を調べます。
指定座標のHEXが指定したHEX種別と等しい場合にイベント発生。
HEX種別に11,16を指定した場合、pvalに艦隊の覇王IDが設定されます。

HEX種別
  0  フリーエリア
  1  小惑星
  2  ブラックホール
  3  恒星
  4  航行不可
  5  惑星
  11 艦隊
  16 要塞

例）指定座標10,13は惑星である
if_mappos,10,13,5
- rai7,if

# if_player_item,アイテム名:ItemName
プレイヤーが指定のアイテムを持っているか？

例）プレイヤーがチロシンを持っている場合、イベント発生。
if_player_item,チロシン
- rai7,if

# if_player_item,アイテム名:ItemName,ランク:Choice|S1|S2|S3|S4|S5|SS|T1|T2|T3|T4|T5|T10
プレイヤーが指定のアイテムを持っているか？
- rai8,if

# if_target_wid,覇王ID:RulerId,惑星ID:PlanetId
指定覇王の攻撃目標が惑星IDと一致する場合にイベントが発生します。
- rai7,if

# if_target_hao,覇王ID1:RulerId,覇王ID2:RulerId
覇王ID1の攻撃目標が覇王ID2の惑星になっている場合にイベントが発生します。
- rai7,if

# if_cnt_habatutyo,覇王ID:RulerId,(<>!)派閥数:PredicateValue@Int
指定の覇王に存在する派閥の数を比較します。
- rai7,rai8,if

# if_cnt_habatumem,派閥長ID:FactionId,(<>!)メンバー数:PredicateValue@Int
指定の派閥に属する派閥メンバー数を比較します。
- rai7,rai8,if

# if_exist_fam,人物ID:PersonId,関係文字列:Choice|母|父|兄|姉|妻|妹|弟|息子|娘
人物IDに指定の関係文字列に対応する人物がいるか調べます。
存在する場合はイベントが発生し、pvalに検索結果の人物IDを設定します。
検索結果が複数の場合は任意の1名がpvalにセットされます。

関係文字列で指定可能な文字
母 父 兄 姉 妻 妹 弟 息子 娘

例）pid1に設定されている人物IDの兄が存在するかチェックし、pid2に設定します。
if_exist_fam,pid1,兄
set_epid2,pval
- rai7,rai8,if

# if_val,判定条件:Predicate
判定条件に、さまざまな指定をすることで、いろいろな使い方ができます。
判定条件には、=  >=  <=  >  <  ! の比較条件を指定できます。
if_valは、人物、惑星ステータスの間接指定と組み合わせることによって、
従来の多くのコマンドをif_valで処理することができます。

例）
if_val,妻:player>0　　    -> プレイヤーは結婚しているか？
if_val,資金:31>=10000     -> 覇王ID31の資金は10000以上あるか？
if_val,知力:645>知力:756  -> 人物ID645の知力が756の知力より上か？
if_val,勲功:332!12300     -> 人物ID332は王子ではない
if_val,状態:pid2=3　　    -> pid2の人物IDは死亡しているか？
if_val,役職:player=2      -> プレイヤーは艦隊総司令であるか？
if_val,初膜:233=0         -> ID233は処女であるか？
if_val,領王:14=poid_player -> 惑星ID14はプレイヤー覇王が支配しているか？
if_val,eveflg088=100      -> イベントフラグ88の値が100であるか？
if_val,覇王:23=156        -> 人物ID23は覇王ID156の部下であるか？
if_val,弱み:256>0         -> 人物ID256の弱みを握っているか？
- rai7,rai8,if

# if_orval,判定条件1:Predicate?,判定条件2:Predicate?,判定条件3:Predicate?,判定条件4:Predicate?,判定条件5:Predicate?
if_valの判定条件を５つまで論理和でチェックすることができます。

例）人物ID512はプレイヤーである　or 人物ID512は覇王である
if_orval,player=512,勲功:512=13000,,,
- rai7,rai8,if

# if_is_habatu,覇王ID:RulerId
指定の覇王に派閥が１つ以上存在する場合にイベントが発生します。
- rai7,rai8,if

# if_kan_sirei,人物ID:PersonId
指定の人物IDは艦隊総司令もしくは艦隊司令である場合にイベントが発生します。
- rai7,if

# if_yaku23,人物ID:PersonId
指定の人物IDは艦隊総司令もしくは艦隊司令である場合にイベントが発生します。
旧コマンドです。代わりにif_kan_sireiを使ってください。
- rai7,if,deprecated

# if_hcnt,(<>!)覇王数:PredicateValue@Int
全体の覇王数をチェックします。

例）覇王が11人以上いればイベント発生
if_hcnt,>10
- rai7,rai8,if

# if_ouzoku,覇王ID:RulerId,勲功:Honor,除外人物ID:PersonId
指定覇王に王子や姫がいるかチェックします。
除外人物IDに0以外を指定すると、その人物はチェック対象外となります。

例）プレイヤー所属覇王に王子がいるか？　ただしID511はのぞく
if_ouzoku,poid_player,12300,511
- rai7,rai8,if

# if_year,暦年:Year
指定の年ならイベントが発生します。
- rai7,if

# if_month,月:Month|1|2|3|4|5|6|7|8|9|10|11|12
指定の月ならイベントが発生します。

例）毎年6月にイベント発生
if_month,6
- rai7,rai8,if

# if_amon,月:Month|1|2|3|4|5|6|7|8|9|10|11|12
指定の月になるとイベント発生、毎年その月になると発生します。
旧コマンドです。代わりにif_monを使ってください。
- rai7,if,deprecated

# if_chkwsts,惑星ID:PlanetId,惑星ステータスID:PlanetStatusId,(<>!)判定値:PredicateValue@ReferPrevStatusId
惑星IDのステータスと判定値を比較し条件が一致する場合にイベントが発生します。
例）惑星ID14の民忠が80以上あるか？
if_chkwsts,14,民忠,>79
- rai7,if

# if_pbuka,部下人物ID:PersonId,プレイヤーID:PersonId
部下人物IDがプレイヤーの部下である場合にイベントが発生します。

例）852がプレイヤーの部下か
if_pbuka,852,player
- rai7,rai8,if

# if_aft_date,暦年:Year,月:Month|1|2|3|4|5|6|7|8|9|10|11|12
指定した暦年月以降(指定年月を含む)であればイベントが発生します。
- rai7,rai8,if

# if_bef_date,暦年:Year,月:Month|1|2|3|4|5|6|7|8|9|10|11|12
指定した暦年月以前(指定年月を含む)であればイベントが発生します。
- rai7,rai8,if

# if_date,暦年:Year,月:Month|1|2|3|4|5|6|7|8|9|10|11|12
指定した年月になった場合、イベントが発生します。
- rai7,rai8,if

# if_same_oid,人物ID1:PersonId,人物ID2:PersonId
指定の人物が同じ覇王の配下にいる場合、イベントが発生します。
- rai7,if

# if_not_same_oid,人物ID1:PersonId,人物ID2:PersonId
指定の人物が同じ覇王の配下にいない場合、イベントが発生します。
- rai7,if

# if_uni,覇王ID1:RulerId,覇王ID2:RulerId
指定の覇王が同盟していればイベントが発生します。
- rai7,if

# if_not_uni,覇王ID1:RulerId,覇王ID2:RulerId
指定の覇王が同盟していればイベントが発生します。
- rai7,if

# if_rnd,乱数値:Int
乱数を発生させ、1が出た場合　イベントが発生します。
確率によりイベントを発生させます。
1/乱数値　という確率になります。
乱数値は2以上を指定してください。
- rai7,rai8,if

# if_yaku,人物ID:PersonId,役職ID:JobId
指定人物が指定の役職にある場合にイベントが発生します。
- rai7,if

# if_not_yaku,人物ID:PersonId,役職ID:JobId
指定人物が指定の役職ではない場合にイベントが発生します。
- rai7,if

# if_kainin,人物ID:PersonId,(<>!)妊娠月数:PredicateValue@PregnancyMonth
人物IDの妊娠月を比較して、条件を満たす場合にイベントが発生します。
妊娠月数には以下の定数
  TANJO : 産まれる月
  PTANJO: 産まれる月の1ヶ月前
が指定できます。

例）プレイヤーの嫁の妊娠月数が10ヶ月目である場合
if_kainin,yome_player,10
- rai7,rai8,if

# if_pid1,(<>!)値:PredicateValue@PersonId
pid1の内容を比較します。
※このコマンドは if_val にて代用できます。
- rai7,rai8,if,deprecated

# if_pid2,(<>!)値:PredicateValue@PersonId
pid2の内容を比較します。
※このコマンドは if_val にて代用できます。
- rai7,rai8,if,deprecated

# if_pid3,(<>!)値:PredicateValue@PersonId
pid3の内容を比較します。
※このコマンドは if_val にて代用できます。
- rai7,rai8,if,deprecated

# if_pid4,(<>!)値:PredicateValue@PersonId
pid4の内容を比較します。
※このコマンドは if_val にて代用できます。
- rai7,rai8,if,deprecated
 
# if_pval,(<>!)値:PredicateValue@PersonId
pvalの内容を比較します。
※このコマンドは if_val にて代用できます。
- rai7,rai8,if,deprecated

# if_cpsts,ステータスID:PersonStatusId,人物ID1<>!=人物ID2:Compare@PersonId
※このコマンドは if_val にて代用できます。
人物ID1と人物ID2のステータスを比較し条件が一致する場合にイベントが発生します。

例）ID211とID512の性別が等しい場合
if_cpsts,性別,211=512
- rai7,rai8,if,deprecated

# if_kaizoku_free,海賊の人物ID:PirateId
指定の海賊人物IDが契約中でない場合にイベントが発生します。
- rai7,if

# if_kaizoku_not_free,海賊の人物ID:PirateId
指定の海賊人物IDが契約中の場合にイベントが発生します。
- rai7,if

# if_rndwman,仕官中:Choice|on|off,村娘:Choice|on|off,放浪:Choice|on|off,死亡:Choice|on|off,年齢:Age,仕官モード:Choice|0:プレイヤー勢力の女は含まない|1:プレイヤー勢力の女も含む|2:プレイヤー勢力の女のみ
女人物IDが取得できるか？　を事前に確認します。
取得できる女人物がいればイベントが発生します。。
get_rndwmanを実行する場合の事前確認用スクリプト。
取得可能な女人物IDを pval に設定します。

仕官中：仕官中の女IDを含める場合はon,含めない場合はoffを設定
村娘,放浪,死亡→同上にon/offを設定する
年齢：設定値以上の年齢が対象
仕官モード：仕官中がONの場合に有効なパラメータ
  0:プレイヤー勢力の女は含めない
  1:含める
  2:プレイヤー勢力の女のみが対象

例）仕官中（自国の女以外）、村娘、放浪中で12歳以上の女からランダムで人物IDがとれるか確認します。
eve_start
    if_rndwman,on,on,on,off,12,0
    //取得した人物IDをpid2に設定する
    set_epid2,pval
eve_end
- rai7,rai8,if

# if_money,覇王ID:RulerId,(<>!)資金:PredicateValue@Int
指定覇王の資金が条件を満たす場合はイベントが発生します。
- rai7,rai8,if

# if_cnt_yaku,覇王ID:RulerId,役職ID:JobId,(<>!)人数:PredicateValue@Int
指定覇王の指定担当の人数が条件を満たす場合はイベントが発生します。

例）プレイヤー覇王の妾数が5人以上の場合
if_cnt_yaku,poid_player,3,>4
- rai7,rai8,if

# if_chksts,人物ID:PersonId,ステータスID:PersonStatusId,(<>!)判定値:PredicateValue@ReferPrevStatusId
※このコマンドはif_valで代用できます。
人物ステータスの判定を行い、判定値がtrueなら、イベントが発生します。
- rai7,rai8,if,deprecated

# if_target,覇王ID:RulerId,惑星ID:PlanetId
指定覇王が指定の惑星を攻撃目標にしている場合にイベントが発生します。
- rai7,rai8,if

# if_haou,人物ID:RulerId
指定覇王が覇王ならイベントが発生します。
- rai7,rai8,if

# if_not_haou,人物ID:RulerId
指定覇王が覇王でない場合にイベントが発生します。
- rai7,rai8,if

# if_cmpos,X:Int,Y:Int
指定座標XYに惑星の移動が可能であるか、確認します。
移動可能な場合はイベントが発生します。
set_wpos、put_wakuseiを実行する前にこのコマンドで事前確認しておくことを推奨します。
- rai7,if

# if_cwpos,惑星ID:PlanetId
指定惑星座標に機動艦隊がいない場合にイベントが発生します。
- rai7,if

# if_cfpos,X:Int,Y:Int,覇王ID:RulerId
指定座標X,Yに指定覇王の要塞が建設可能な場合にイベントが発生します。
- rai7,if

# if_wexist,惑星ID:PlanetId
指定惑星が出現していればイベントが発生します。
- rai7,if

# if_not_wexist,惑星ID:PlanetId
指定惑星が出現していなければイベントが発生します。
- rai7,if

# if_ncwid,覇王ID:RulerId
指定覇王は惑星を2つ以上持っている場合にイベントが発生します。
- rai7,if

# if_habatutyo,派閥長ID:FactionId
指定の人物が派閥長である場合にイベントが発生します。
- rai7,if

# if_habatuper,派閥長ID:FactionId,(<>!)派閥勢力割合(%):PredicateValue@Int
指定の派閥長の派閥が派閥勢力割合の条件を満たす場合はイベントが発生します。
派閥勢力割合は0-100(%)で指定
- rai7,if

# if_fltonwid,惑星ID:PlanetId,覇王ID:RulerId
指定惑星の座標に指定覇王の機動艦隊がいる場合はイベントが発生します。
- rai7,if

# if_per_wid,覇王ID:RulerId,(<>!)惑星支配率(%):PredicateValue@Int
指定覇王の惑星支配率が条件を満たす場合はイベントが発生します。
惑星支配率は0-100(%)で指定
- rai7,if

# if_cnt_wid,覇王ID:RulerId,(<>!)占領惑星数:PredicateValue@Int
指定覇王の惑星数が条件を満たす場合はイベントが発生します。
- rai7,if

# if_pnen,人物ID:PersonId,(<>!)年齢値:PredicateValue@Age
指定人物の年齢が条件を満たす場合はイベントが発生します。
年齢値は文字列 OLD を指定できます。
OLD はシステムで設定されているオルド可能年齢です。
- rai7,if

# if_sizitu,史実モード:Choice|on|off
史実モードがオン/オフの場合にイベントが発生します。
- rai7,if

# if_extson,覇王ID:RulerId,人物ID1:PersonId,人物ID2:PersonId,人物ID3:PersonId,人物ID4:PersonId
覇王に人物ID1～4以外の王子がいる場合にイベントが発生します。
チェックする人数が4人未満の場合は、人物IDに0を指定してください。
- rai7,if

# if_atkable,惑星ID:PlanetId,覇王ID:RulerId
指定の覇王が指定の惑星を攻略可能ならイベントが発生します。
- rai7,if

# if_not_atkable,惑星ID:PlanetId,覇王ID:RulerId
指定の覇王が指定の惑星を攻略不可能ならイベントが発生します。
- rai7,if

# if_sirei_aki,ID:PersonId
- rai7,if

# if_enefltonwid,惑星ID:PlanetId
指定された惑星IDに敵艦隊がいる場合にイベントが発生します。
- rai7,if

# if_virgin,人物ID:PersonId
指定の人物が処女の場合にイベント発生
- rai7,if

# if_not_virgin,人物ID:PersonId
指定の人物が非処女の場合にイベント発生
- rai7,if

# if_single,人物ID:PersonId
指定の人物が独身の場合にイベント発生
- rai7,if

# if_not_single,人物ID:PersonId
指定の人物が既婚の場合にイベント発生
- rai7,if

# if_cap,惑星ID:PlanetId,覇王ID:RulerId
指定の惑星が指定の覇王の首都である場合にイベント発生
- rai7,if

# if_not_cap,惑星ID:PlanetId,覇王ID:RulerId
指定の惑星が指定の覇王の首都でない場合にイベント発生
覇王IDが0の場合、覇王のチェックは行わない。
- rai7,if

# if_hid,惑星ID:PlanetId,覇王ID:RulerId
指定の惑星が指定の覇王の惑星である場合にイベント発生
- rai7,if

# if_not_hid,惑星ID:PlanetId,覇王ID:RulerId
指定の惑星が指定の覇王の惑星でない場合にイベント発生
- rai7,if

# if_not_moving,人物ID:PersonId
指定の人物が移動中でない場合にイベント発生
- rai7,if

# if_player,人物ID:PersonId
指定の人物がプレイヤーの場合にイベント発生
- rai7,if

# if_not_player,人物ID1:PersonId,人物ID2:PersonId?
指定の人物がプレイヤーでない場合にイベント発生
1人しかチェックしない場合は、人物ID2に0を指定してください。
- rai7,if

# if_horyo,人物ID:PersonId,覇王ID:RulerId
指定の人物が指定の覇王の捕虜の場合、イベント発生
- rai7,rai8,if

# if_poid,人物ID:PersonId,覇王ID:RulerId
指定の人物が指定覇王の配下の場合、イベント発生
- rai7,rai8,if

# if_not_poid,人物ID:PersonId,覇王ID:RulerId
指定の人物が指定覇王の配下にいない場合、イベント発生
- rai7,if

# if_pkun,人物ID:PersonId,(<>!)勲功:PredicateValue@Honor
指定の人物の勲功が条件を満たす場合、イベント発生
- rai7,if

# if_psex,人物ID:PersonId,性別:Choice|1:男|2:女
指定の人物が指定の性別の場合、イベント発生

性別
  1:男
  2:女
- rai7,if

# if_psts,人物ID:PersonId,状態ID:SituationId
人物IDの状態ステータスが指定した状態ステータスと等しい場合、イベント発生
- rai7,rai8,if

# if_born,人物ID:PersonId,月:Int|1|2|3|4|5|6|7|8|9|10|11|12
指定人物の出産する指定ヵ月前に、イベント発生
- rai7,rai8,if

# if_eveflg,フラグNO:EveFlgNo,値:Int
指定イベントフラグが指定値と等しい場合、イベント発生
- rai7,rai8,if

# if_not_eveflg,フラグNO:EveFlgNo,値:Int
指定イベントフラグが指定値と異なる場合、イベント発生
- rai7,rai8,if

# if_bet_eveflg,フラグNO:EveFlgNo,最小値:Int,最大値:Int
指定イベントフラグが最小値以上最大値以下の場合、イベント発生
- rai7,rai8,if

# if_enearea,主星ID:PrimaryPlanetId,覇王ID:RulerId
惑星系内に敵の建設エリアがある場合にイベント発生。
覇王IDが1以上の場合、惑星IDの覇王が指定の覇王以外の場合は条件不成立。
覇王IDが0の場合、覇王のチェックは行わない。
- rai8,if

# if_fixocu,恒星系ID:StarId,覇王ID:RulerId
恒星系内に指定覇王の惑星がある場合にイベント発生。
- rai8,if

# if_poid_sts2,人物ID:PersonId,覇王ID:RulerId
指定の人物IDが指定覇王IDの配下にいる場合にイベント発生。
ただし諜報中および探査出撃中は条件不成立。惑星滞在中のみ。
- rai8,if

# if_unispc
同盟数（最大６同盟）に空き枠がある場合にイベント発生。
- rai8,if

# if_loop_cnt
- rai8,if

# if_hmode,p1:Choice|on|off|
- rai8,if
 
# add_seikin,精勤ポイント:Int
プレイヤーの精勤ポイントを増減します。

例）精勤ポイント+1
add_seikin,1
- rai7,rai8,command

# fship_sw,旗艦ID:ShipId,スイッチ:Choice|0:非表示|1:表示
旗艦変更コマンドで旗艦候補の一覧に表示するか、しないかを設定します。
表示に設定しても、表示の上限に達している場合は表示されません。

スイッチ
  0:非表示
  1:表示
- rai7,command

# get_union_member,覇王ID:RulerId,モード:Choice|0:盟主の覇王|1:同盟国の覇王|2:従属同盟の覇王
指定覇王と同盟している覇王のIDを得てpvalに設定します。みつからない場合は0がセットされます。
モード
  0:同盟盟主の覇王IDを得る
  1:同盟国の中からランダムで他の参加覇王IDを得る（従属同盟の覇王も含む）
  2:同盟国の中で従属同盟である覇王IDを得る
- rai7,command

# get_union_meisyu,除外盟主覇王ID1:RulerId,除外盟主覇王ID2:RulerId,除外盟主覇王ID3:RulerId
指定の盟主覇王ID以外の同盟盟主覇王IDを得てpvalに設定します。みつからない場合は0がセットされます。
除外盟主覇王IDは３人まで指定可能。指定しない場合は0を設定します。
除外盟主覇王IDは盟主覇王でない人物IDを指定してもエラーにはなりません。

例）複数の同盟が存在する場合に、覇王ID=16以外の同盟盟主覇王IDを得る
get_union_meisyu,16,0,0
dbg_print,pval=,pval
- rai7,command

# zin_anime_init,ファイル名:File,種別:Choice|jpg|png|bmp,最終連番:Int,幅:Int,高さ:Int,X:Int,Y:Int,フレームレート:Int,非同期フラグ:Choice|0:同期|1:非同期
オルド画像のアニメーションを制御します。
- rai7,rai8,command

# zin_anime_start
オルド画像のアニメーションの非同期時の再生開始コマンド。同期モードでは不要。
- rai7,rai8,command

# zin_anime_stop
オルド画像のアニメーションの非同期時の再生停止コマンド。
- rai7,rai8,command

# div,フラグNO:EveFlgNo,割られる数:Int,割る数:Int
指定イベントフラグに割られる数を割る数で割った結果を設定します。
- rai7,rai8,command

# mul,フラグNO:EveFlgNo,p1:Int,p2:Int
指定イベントフラグにp1とp2を掛けた結果を設定します。
- rai7,rai8,command

# plus,フラグNO:EveFlgNo,p1:Int,p2:Int
指定イベントフラグにp1とp2を足した結果を設定します。
- rai7,rai8,command

# sub,フラグNO:EveFlgNo,引かれる数:Int,引く数:Int
指定イベントフラグに引かれる数から引く数を引いた結果を設定します。
- rai7,rai8,command

# set_whanyo,惑星ID:PlanetId,値:Int
指定惑星の惑星汎用ステータスに値を設定します。
- rai7,command

# set_wepkei,p1:Any,p2:Any
- rai7,command

# expl_pos,X:Int,Y:Int
指定座標の艦隊および要塞を消滅させます。
- rai7,command

# set_event_off
標準のドラマイベント(eNNN.txt)の起動を停止します。
ユーザーイベント(uNNN.txt)の実行には影響しません。

※ana.txtの
//機能設定はモードに関係なく以下の設定を有効にする　←offにしてください。
- rai7,command

# set_target,覇王ID:RulerId,惑星ID:PlanetId
指定覇王の攻撃目標を惑星IDに設定します。
- rai7,rai8,command

# set_kmdata,目標覇王ID:RulerId,覇王ID1:RulerId,覇王ID2:RulerId,覇王ID3:RulerId
覇王2,3,4の攻撃目標を目標覇王に設定します。
- rai7,rai8,command

# set_kasi,人物ID:PersonId
指定の人物IDを仮死状態（状態ステータス=6）にします。
- rai7,rai8,command

# set_syugyo,人物ID:PersonId
指定の人物IDを修行状態（状態ステータス=9）にします。
- rai7,command

# set_kaizoku,人物ID:PersonId
指定の人物IDを海賊のカシラ（状態ステータス=4）にします。
- rai7,command

# set_wpos,惑星ID:PlanetId,X:Int,Y:Int 
指定の惑星を移動先座標X,Yに移動します。
移動先が移動不可HEXの場合、移動はキャンセルされます。
if_cwpos で移動可能か確認できます。
- rai7,command

# wexpl,惑星ID:PlanetId
指定の惑星を破壊します。
惑星HEX上にいる艦隊はダメージを受けます。
首都星を破壊すると、その勢力は消滅します。
- rai7,command

# drop_item,人物ID:PersonId,アイテム名:ItemName
指定した人物IDのアイテムを破棄します。

例）プレイヤーが持つチロシンを破棄する。
drop_item,player,チロシン
- rai7,command

# goto,*ラベル名:Label
指定ラベルへジャンプします

例）
goto,*label1
  zin_reg_msg,おい、貴様,1
  zin_reg_msg,うん？　誰だお前は？,0
*label1
  zin_reg_msg,貴様、何者だ！！,0
  zin_reg_msg,フフ、そんな腕で勝てるのか？,1
- rai7,rai8,command,control

# gosub,*ラベル名:Label
ラベル名のサブルーチンをコールします。
return文で、コール元に戻ります。
gosubコマンドはネスト（サブルーチンの内部でさらにgosubを呼ぶ）が可能です。
例）
zin_reg_msg,１１１１,0
gosub,*sub1
zin_reg_msg,５５５５,0
zin_reg_button,0
end

*sub1
  zin_reg_msg,２２２２,0
  gosub,*sub2
  zin_reg_msg,４４４４,0
  return

*sub2
  zin_reg_msg,３３３３,0
  return
- rai7,rai8,command,control

# return
gosubコマンドのコール元に戻ります。
- rai7,rai8,command,control

# txtload,イベントファイル名:ScriptFile
他のイベントファイルへジャンプします。
イベントファイル名に拡張子.txtは不要。
- rai7,rai8,command,control

# set_pbuka,人物ID:PersonId,年齢:Age
指定した人物IDをプレイヤーの部下に設定します。
人物IDの年齢を再設定することができます。
現在の年齢を変更しない場合は0を指定します。
覇王は部下を持てないので、事前にプレイヤーが覇王ではないことをif_not_haou等で確認しておく必要があります。

例）
eve_start
  if_not_haou,player
eve_end
//人物ID522をプレイヤーの部下に設定
set_pbuka,522,0
- rai7,rai8,command

# switch_otaii,人物ID:PersonId
オルドCGの体位ごとに処理を分岐します。

case_oseijo   : 正常位
case_oback    : バック
case_okijo    : 騎乗位
default_otaii : デフォルト処理

*switch_otaii_endを記述するとそこで分岐は終了します。

例）
switch_otaii,pid2
case_oback
//バックの台詞
    goto,*switch_otaii_end
case_okijo
//正常位の台詞
    goto,*switch_otaii_end
default_otaii
//その他体位の台詞
*switch_otaii_end
- rai7,command,control

# case_oseijo
switch_otaiiコマンドの正常位の場合のジャンプ先
- rai7,command,control

# case_oback
switch_otaiiコマンドのバックの場合のジャンプ先
- rai7,command,control

# case_okijo
switch_otaiiコマンドの騎乗位の場合のジャンプ先
- rai7,command,control

# default_otaii
switch_otaiiコマンドのデフォルトの場合のジャンプ先
- rai7,command,control

# get_mpid,判定条件1:GetMpid,判定条件2:GetMpid?,判定条件3:GetMpid?,判定条件4:GetMpid?,判定条件5:GetMpid?,判定条件6:GetMpid?,判定条件7:GetMpid?
判定条件にマッチした人物IDを pval に設定します。
各条件は論理積(and)となります。
マッチした人物が複数いた場合は、ランダムで1人が選択されます。
マッチした人物がいない場合はpvalに0が設定されます。

判定条件は　ステータスID=値　の形で設定されます。
判定条件には、= >= <= > < ! の比較演算子を指定できます。

ステータスIDが「状態」の場合で　比較演算子が = の場合に限り、値を | 区切りで
複数設定することができます。
例） 状態=5|7|18
この場合、各状態IDは論理和(or)でチェックされます。

例）状態は放浪or村娘or平民 and 性別は女 and 格闘が40以上の人物をpid3へ設定する。
get_mpid,状態=5|7|18,性別=2,格闘>=40,,,,
set_epid3,pval
- rai7,rai8,command

# psiriai,プレイヤーID:PersonId,人物ID:PersonId,親密値:Int
指定の人物IDの面会回数を+1増加し、親密値を設定します。
- rai7,command

# habatu_kaitai,派閥長ID:FactionId
指定派閥長の派閥を解体します。
- rai7,rai8,command

# hba_bunretu,p1:PersonId,p2:Any,p3:Any
- rai7,command

# set_fammov,p1:PersonId,p2:PersonId
- rai7,command

# switch_seikaku,人物ID:PersonId  
人物の性格によって処理を分岐します。
*switch_seikaku_endを記述すると、処理はそこで終了します。

例）
//ID533の性格を判定
switch_seikaku,533
case_seikaku11
  //性格値11の場合の処理
  goto,*switch_seikaku_end
case_seikaku12
  //性格値12の場合の処理
  goto,*switch_seikaku_end
default_seikaku
  //その他性格の処理
*switch_seikaku_end
- rai7,rai8,command,control

# case_seikaku11
switch_seikakuコマンドの性格がトラの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku12
switch_seikakuコマンドの性格がサルの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku13
switch_seikakuコマンドの性格がコアラの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku14
switch_seikakuコマンドの性格がオオカミの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku21
switch_seikakuコマンドの性格がペガサスの場合のジャンプ先
- rai7,rai8,command,command,control

# case_seikaku22
switch_seikakuコマンドの性格がゾウの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku23
switch_seikakuコマンドの性格がライオンの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku24
switch_seikakuコマンドの性格がチータの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku31
switch_seikakuコマンドの性格が黒ヒョウの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku32
switch_seikakuコマンドの性格がヒツジの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku41
switch_seikakuコマンドの性格がコジカの場合のジャンプ先
- rai7,rai8,command,control

# case_seikaku42
switch_seikakuコマンドの性格がタヌキの場合のジャンプ先
- rai7,rai8,command,control

# default_seikaku
switch_seikakuコマンドのデフォルトの場合のジャンプ先
- rai7,rai8,command,control

# off_habatu,人物ID:PersonId
指定の人物を派閥から脱退させます。
人物IDに派閥長を指定した場合は無視されます。
- rai7,rai8,command

# set_habatusts,派閥長ID:FactionId,状態ID:SituationId
指定派閥の人物の状態を設定します。
- rai7,rai8,command

# bak_habatusts,人物ID1:PersonId,人物ID2:PersonId
- rai8,command

# mov_kantei_kasi,p1:RulerId,p2:RulerId
- rai8,command

# mov_kantei_allback,p1:RulerId,p2:RulerId
- rai8,command

# set_yousi,子人物ID:PersonId,親人物ID:PersonId
子人物IDを親人物IDの養子に設定します。
- rai7,command

# chk_mval,判定条件1:Predicate?,判定条件2:Predicate?,判定条件3:Predicate?,判定条件4:Predicate?,判定条件5:Predicate?
全ての判定条件が満たされる場合、chk_mval_then、満たされない場合chk_mval_elseにジャンプします。
判定条件には、= >= <= > < ! の比較演算子を指定できます。

人物、惑星ステータスの間接指定と組み合わせることによって、従来の多くのコマンドをchk_mvalで処理できます。

例）プレイヤーのIDが512である and pid1の勲功値がpid2の勲功値より大きい
chk_mval,player=512,勲功:pid1>勲功:pid2,,,
chk_mval_then
    //条件が満たされる場合の処理
    //処理の最後にgotoコマンドかendを記述しないと、そのままchk_mval_else
    //の処理が実行されます。
chk_mval_else
    //条件が満たされない場合の処理
- rai7,rai8,command,control

# chk_mval_then
chk_mvalの条件が満たされた場合のジャンプ先
- rai7,rai8,command,control

# chk_mval_else
chk_mvalの条件が満たされなかった場合のジャンプ先
- rai7,rai8,command,control

# chk_mval_end
- rai8,command,control

# chk_ormval,判定条件1:Predicate?,判定条件2:Predicate?,判定条件3:Predicate?,判定条件4:Predicate?,判定条件5:Predicate?
判定条件が1つでも満たされる場合、chk_ormval_then、全て満たされない場合chk_ormval_elseにジャンプします。
判定条件には、= >= <= > < ! の比較演算子を指定できます。
- rai7,rai8,command,control

# chk_ormval_then
chk_ormvalの条件が満たされた場合のジャンプ先
- rai7,rai8,command,control

# chk_ormval_else
chk_ormvalの条件が満たされなかった場合のジャンプ先
- rai7,rai8,command,control

# chk_ormval_end
- rai8,command,control

# get_bgst_habatu,覇王ID:RulerId,除外人物ID:PersonId
指定覇王の勢力で、最大派閥長の人物IDを pval に設定します。
除外人物IDを設定すると、その人物は検索対象外になります。未設定の場合は0を指定。
最大派閥長が存在しない場合はpvalに0が設定されます。
- rai7,rai8,command

# get_phao
プレイヤーの覇王IDを pval に設定します。
プレイヤーが仕官中でない場合、適当な覇王IDがpvalにセットされます。
- rai7,command

# cls_allsp2
特殊設定が2の人物をすべて初期化(0)します。
- rai7,rai8,command

# get_mwpid
現在、最大の惑星数を持つ覇王IDを pval に設定します。
- rai7,rai8,command

# chk_existpid,人物ID1:PersonId,人物ID2:PersonId
人物ID1と人物ID2が同じ覇王勢力に所属しているかをチェックします。
同じ勢力にいる場合は、chk_existpid_then、同じ勢力にいない場合は、chk_existpid_elseにジャンプします。

例）ID312とID65は同じ覇王の部下であるか？
chk_existpid,312,65
chk_existpid_then
    //同じ覇王の場合の処理
    //処理の最後にgotoコマンドかendを記述しないと、そのままchk_existpid_else
    //の処理が実行されます。
chk_existpid_else
    //覇王が異なる場合の処理
- rai7,command,control

# chk_existpid_then
chk_existpidの条件が満たされた場合のジャンプ先
- rai7,rai8,command,control

# chk_existpid_else
chk_existpidの条件が満たされなかった場合のジャンプ先
- rai7,rai8,command,control

# chk_exist_fam,人物ID1:PersonId,p2:Any
- rai7,rai8,command,control

# chk_exist_fam_then
- rai7,rai8,command,control

# chk_exist_fam_else
- rai7,rai8,command,control

# owner_sel,選択肢文字列1:Speech,選択肢文字列2:Speech,覇王ID:RulerId
2択のダイアログを表示します。
ただし、指定覇王IDとプレイヤーの覇王IDが同じ場合のみ表示され異なる場合は、乱数で結果が自動選択されます（選択ダイアログは非表示）。

選択結果は get_resp コマンドで取得します。
  ret_sel1: 選択肢1を選んだ場合
  ret_sel2: 選択肢2を選んだ場合

例）プレイヤーの覇王IDが16の場合のみ、選択ダイアログが表示されます。
owner_sel,選択肢A,選択肢B,16
get_resp
ret_sel1
    //選択肢Aを選んだ場合の処理
    //処理の最後にgotoコマンドかendを記述しないと、そのままret_sel2
    //の処理が実行されます。
ret_sel2
    //選択肢Bを選んだ場合の処理
- rai7,command,control

# btl_ansatu,人物ID1:PersonId,人物ID2:PersonId,モード:Choice|0:死亡しない|1:負けたら死亡,背景:Choice|0:暗殺背景|1:宮廷内仇討ち用|2:部下同士深夜
人物ID1と人物ID2の格闘戦闘イベントを発生させます。

モード
  0: 負けても死亡しない 
  1: 負けた人物は死亡する

背景
  0: 暗殺背景
  1: 宮廷内仇討ち用
  2: 部下同士 深夜

戦闘結果は get_resp コマンドで取得します。
  ret_sel2: 人物ID1の勝利
  ret_sel3: 人物ID2の勝利


例）ID852とID762で格闘戦を行う。負けても死亡しません。背景は深夜。
btl_ansatu,852,762,0,2
get_resp
ret_sel2
    //ID852が勝利した場合の処理
    //処理の最後にgotoコマンドかendを記述しないと、そのままret_sel3
    //の処理が実行されます。
ret_sel3
    //ID762の勝利
- rai7,rai8,command,control

# btl_kiriai,人物ID1:PersonId,人物ID2:PersonId
- rai8,command,control

# hao_ansatu,覇王ID:RulerId,人物ID:PersonId
指定人物が指定覇王の暗殺イベントを発生させます。
特命担当が存在すれば、特命担当も戦闘します。

戦闘結果は get_resp コマンドで取得します。
  ret_sel1: 覇王の勝利
  ret_sel2: 覇王の敗亡
- rai7,rai8,command,control

# set_kousai,人物ID:PersonId
指定の人物IDの親密値を交際中に設定します。
- rai7,command

# set_wakare,交際中の人物ID:PersonId
交際中の人物と別れます。親密値は1に設定されます。
- rai7,command

# add_menkai,人物ID:PersonId
プレイヤーとの面会回数を１つ増やします。
- rai7,command

# set_datai,妊娠中の人物ID:PersonId
妊娠中の人物を中絶させます。
- rai7,rai8,command

# chk_kunko,人物ID1:PersonId,人物ID2:PersonId
人物ID1と人物ID2の階級を比較し、上司や部下といった処理を分岐することができます。

case_kun_同僚: 人物1が人物2の同僚の場合
case_kun_部下: 人物1が人物2の部下の場合
case_kun_上司: 人物1が人物2の上司の場合
default_kun : デフォルトの場合
- rai7,rai8,command,control

# case_kun_同僚
chk_kunkoコマンドの人物1が人物2の同僚の場合のジャンプ先
- rai7,rai8,command,control

# case_kun_部下
chk_kunkoコマンドの人物1が人物2の部下の場合のジャンプ先
- rai7,rai8,command,control

# case_kun_上司
chk_kunkoコマンドの人物1が人物2の上司の場合のジャンプ先
- rai7,rai8,command,control

# default_kun
chk_kunkoコマンドのデフォルトの場合のジャンプ先
- rai7,rai8,command,control

# set_hwid,人物ID:PersonId,惑星ID:PlanetId
指定の惑星を指定人物の領土にします。
通常、人物IDには派閥長を指定します。
- rai7,rai8,command

# set_epid1,人物ID:PersonId
pid1に人物IDを設定します。
このコマンドは発生条件ブロック内でも使用することができます。
- rai7,rai8,command,if

# set_epid2,人物ID:PersonId
pid2に人物IDを設定します。
このコマンドは発生条件ブロック内でも使用することができます。
- rai7,rai8,command,if

# set_epid3,人物ID:PersonId
pid3に人物IDを設定します。
このコマンドは発生条件ブロック内でも使用することができます。
- rai7,rai8,command,if

# set_epid4,人物ID:PersonId
pid4に人物IDを設定します。
このコマンドは発生条件ブロック内でも使用することができます。
- rai7,rai8,command,if

# set_eveflg,フラグNO:EveFlgNo,値:Int
イベントフラグに値をセットします。
このコマンドは、発生条件ブロック内でも使用することができます。
- rai7,rai8,command,if

# add_eveflg,フラグNO:EveFlgNo,増加値:Int
イベントフラグの値に増加値を加算します。
- rai7,rai8,command

# set_run_msg,メッセージ:Speech
ターン開始時の報告にメッセージを表示します。
- rai7,command

# dbg_print,適当な文字列:Speech,変数:Any
デバック用コマンドです。
変数の内容をウインドのタイトルバーに表示します。
タイトルバーの表示されないフルスクリーン状態では確認できません。

例）pid1の内容を表示します。
set_epid1,599
dbg_print,デバッグ情報1=,pid1

eve_start
  if_pnen,pid1,>TANTO
eve_end

zin_reg_event,test,player,pid1,,gzin8
  zin_reg_msg,おまえも担当に就ける年になったな,0
  dbg_print,デバッグ情報2=,TANTO
end
- rai7,rai8,command

# get_hao_wid,惑星ID:PlanetId
指定の惑星IDを支配する覇王IDを pval に設定します。
- rai7,rai8,command

# chk_mssts,惑星ID:PlanetId,ステータスID1:PlanetStatusId,(<>!)判定値1:PredicateValue@ReferPrevStatusId,ステータスID2:PlanetStatusId?,(<>!)判定値2:PredicateValue?@ReferPrevStatusId,ステータスID3:PlanetStatusId?,(<>!)判定値3:PredicateValue?@ReferPrevStatusId,ステータスID4:PlanetStatusId?,(<>!)判定値4:PredicateValue?@ReferPrevStatusId,ステータスID5:PlanetStatusId?,(<>!)判定値5:PredicateValue?@ReferPrevStatusId
※このコマンドはchk_mvalで代用できます。

指定の惑星IDの状態を判定し処理を分岐します。
全ての条件が満たされる場合、chk_mssts_then、満たされない場合chk_mssts_elseにジャンプします。
入れ子には対応していないので、chk_msstsの処理中にchk_msstsを記述するとうまく動作しません。

判定値
  数値の他に大小指定<>!ができます。等しい場合はそのまま数値を記述。

惑星ステータスID
  出現  : 判定値　1:出現している 0:未出現
  首都  : 判定値　1:首都 0:首都以外
  座標X : 判定値  X座標HEXを数値設定
  座標Y : 判定値  Y座標HEXを数値設定
  陸戦数: 
  民忠 ：
  人口 ：
  資金 ：
  世論 ：
  攻撃目標：判定値 惑星IDを設定
  NON  : 条件を設定しない

※資金、世論、攻撃目標を指定した場合、惑星IDは自動的に首都星の情報を参照します。
- rai7,command,control,deprecated

# chk_mssts_then
chk_msstsの条件が満たされた場合のジャンプ先
- rai7,command,control,deprecated

# chk_mssts_else
chk_msstsの条件が満たされなかった場合のジャンプ先
- rai7,command,control,deprecated

# chk_cpsts,ステータスID:PersonStatusId,人物ID1<>!=人物ID2:Compare@PersonId
※このコマンドはchk_mvalで代用できます。

人物ID1と人物ID2のステータスを比較し処理を分岐します。
条件が満たされた場合chk_cpsts_then、満たされない場合chk_cpsts_elseにジャンプします。
入れ子には対応していないので、chk_cpstsの処理中にchk_cpstsを記述するとうまく動作しません。
他のchk_XXstsと異なり、値が等しい場合は=を設定します。

例）ID211とID512の性別が等しい場合
chk_cpsts,性別,211=512
- rai7,command,control,deprecated

# chk_cpsts_then
chk_cpstsの条件が満たされた場合のジャンプ先
- rai7,command,control,deprecated

# chk_cpsts_else
chk_cpstsの条件が満たされなかった場合のジャンプ先
- rai7,command,control,deprecated

# get_rndwman,仕官中:Choice|on|off,村娘:Choice|on|off,放浪:Choice|on|off,死亡:Choice|on|off,年齢:Age,仕官モード:Choice|0:プレイヤー勢力の女は含まない|1:プレイヤー勢力の女も含める|2:プレイヤー勢力のみ
条件にあった女人物IDをランダムにpvalに設定します。
事前に if_rndwman で取得可能か確認しておく必要があります。

仕官中: 仕官中の女IDを含める場合はon,含めない場合はoffを設定
村娘,放浪,死亡: 同上にon/offを設定する
年齢: 設定値以上の年齢が対象
仕官モード: 仕官中がONの場合に有効なパラメータ
  0:プレイヤー勢力の女は含めない 
  1:含める
  2:プレイヤー勢力の女のみが対象

例）仕官中（自国の女以外）、村娘、放浪中で12歳以上の女IDをランダムで取得します。
get_rndwman,on,on,on,off,12,0
set_epid2,pval
- rai7,rai8,command

# get_rndhao,除外覇王ID:RulerId
覇王IDをランダムでpvalに設定します。
除外覇王IDが1以上の場合は、その覇王は対象外になります。
除外覇王IDを設定しない場合は0を指定します。
- rai7,rai8,command

# get_rndwid,除外覇王ID:RulerId
出現している惑星IDをランダムでpvalに設定します。
除外覇王IDが1以上の場合は、その覇王IDが所有する惑星は対象外になります。
除外覇王IDを設定しない場合は0を指定します。

例）惑星IDをランダムで取得します。ただしプレイヤー勢力の惑星以外。
get_rndwid,poid_player
set_eveflg,100,pval
- rai7,command

# get_kaizoku_hao,覇王ID:RulerId
指定の覇王IDと契約中の海賊人物IDを pval に設定します。
- rai7,command

# get_hao_kaizoku,海賊人物ID:PirateId
指定の海賊人物IDと契約中の覇王IDを pval に設定します。
- rai7,command
  
# kaizoku_cont,海賊人物ID:PirateId,覇王ID:RulerId
指定の海賊と覇王で契約を設定します。
- rai7,command

# kaizoku_cancel,海賊人物ID:PirateId
指定の海賊人物IDが契約中の場合、その契約を強制的にキャンセルします。
- rai7,command

# chk_eveflg,フラグNO:EveFlgNo,(<>!)判定値:PredicateValue@Int
指定イベントフラグの値が条件を満たすか判定します。
条件を満たす場合 chk_eveflg_then 満たさない場合 chk_eveflg_else にジャンプします。

判定値に間接指定ができるようになりました。←ver7.66b2以上

例）
イベントフラグ49の値がイベントフラグ50より大きい場合にchk_eveflg_then以降を実行
chk_eveflg,49,>eveflg50
chk_eveflg_then
    //真の処理
chk_eveflg_else
    //偽の処理
- rai7,rai8,command,control

# chk_eveflg_then
chk_eveflg の条件が満たされた場合のジャンプ先
- rai7,rai8,command,control

# chk_eveflg_else
chk_eveflg の条件が満たされなかった場合のジャンプ先
- rai7,rai8,command,control

# chk_family,人物ID1:PersonId,人物ID2:PersonId
人物ID2から見た人物ID1の関係によって処理を分岐します。
default_famは必ずcaseの最後に記述すること。

例)
chk_family,pid2,pid1
case_fam_姉
  //姉の処理
  goto,*tuginosyori
case_fam_母
  //母の処理
  goto,*tuginosyori
case_fam_妻
  //妻の処理
  goto,*tuginosyori
case_fam_娘
case_fam_娘(養)
  //娘の処理
  goto,*tuginosyori
case_fam_妹
  //妹の処理
  goto,*tuginosyori
default_fam
  //家族以外の処理

*tuginosyori
  //buf_str1に続柄文字列（母、姉、娘・・・）が入る
  zin_reg_msg "この人はbuf_str1です",0
- rai7,rai8,command,control

# case_fam_父
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_母
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_夫
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_妻
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_兄
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_弟
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_姉
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_妹
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_義兄
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_義弟
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_義姉
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_姉(義)
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_義妹
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_妹(義)
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_双子
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_息子
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_息子(養)
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_娘
chk_family のジャンプ先
- rai7,rai8,command,control
# case_fam_娘(養)
chk_family のジャンプ先
- rai7,rai8,command,control
# default_fam
chk_family のジャンプ先
- rai7,rai8,command,control

# get_yaku_id,覇王ID:RulerId,役職ID:JobId,除外人物ID1:PersonId,除外人物ID2:PersonId
指定覇王の担当人物IDを pval に設定します。
除外人物IDを設定した場合は、そのID以外の人物IDになります。

※事前にif_cnt_yakuを使用して人数を確認しておくことが必要

例）妾の人物IDを2名得る　→　pid1,pid2にセット
get_yaku_id,poid_player,3,0,0
set_epid1,pval
get_yaku_id,poid_player,3,pid1,0
set_epid2,pval
- rai7,rai8,command

# chk_msts,人物ID:PersonId,ステータスID1:PersonStatusId,(<>!)判定値1:PredicateValue@ReferPrevStatusId,ステータスID2:PersonStatusId?,<>!判定値2:PredicateValue?@ReferPrevStatusId,ステータスID3:PersonStatusId?,(<>!)判定値3:PredicateValue?@ReferPrevStatusId,ステータスID4:PersonStatusId?,(<>!)!判定値4:PredicateValue?@ReferPrevStatusId,ステータスID5:PersonStatusId?,(<>!)判定値5:PredicateValue?@ReferPrevStatusId
※このコマンドはchk_mvalでも代用できます。

指定された人物IDのステータスを判定し、全ての条件が満たされる場合 chk_msts_then, 1つでも満たされない場合は chk_msts_else にジャンプします。
条件を設定しないステータスIDは-1もしくはNONを記述してください。
入れ子には対応していないので、chk_mstsの処理中にchk_mstsを記述するとうまく動作しません。

判定値
  数値の他に大小指定<>ができます。等しい場合はそのまま数値を記述。
  OLD(オルド可能年齢)、TANTO(担当配属可能年齢)、TANJO（妊娠月数）が指定できます。

例）すべての60歳以上の女人物を20歳に若返りさせる
//人物をMAXループ イベントフラグ150番に人物IDが設定される　1からカウント開始
for,PMAX,150,1
  //性別と年齢の条件設定　すべての条件はAND
  chk_msts,eveflg150,性別,2,年齢,>59,NON,0,NON,0,NON,0
  chk_msts_then
    //年齢を20歳に設定
    set_pnen,eveflg150,20
    continue
  chk_msts_else
    continue
next
- rai7,command,control,deprecated

# chk_msts_then
chk_msts の条件が満たされた場合のジャンプ先
- rai7,command,control,deprecated

# chk_msts_else
chk_msts の条件が満たされなかった場合のジャンプ先
- rai7,command,control,deprecated  

# set_pnen,人物ID:PersonId,年齢:Age
指定の人物IDの年齢を設定します。 
- rai7,rai8,command

# wnen_reset
- rai7,command

# chk_sts,人物ID:PersonId,ステータスID:PersonStatusId,(<>!)判定値:PredicateValue@ReferPrevStatusId
※このコマンドはchk_mvalでも代用できます。

指定人物のステータスが条件を満たす場合 chk_sts_then 満たさない場合 chk_sts_else にジャンプします。

判定値
  数値の他に大小指定<>!ができます。等しい場合はそのまま数値を記述。

例）
chk_sts,100,1,>90
chk_sts_then
  //人物ID100の忠誠が90以上の処理
  end
chk_sts_else
  //人物ID100の忠誠が90以下の処理
  end
- rai7,rai8,command,control,deprecated  

# chk_sts_then
chk_sts の条件が満たされた場合のジャンプ先
- rai7,rai8,command,control,deprecated

# chk_sts_else
chk_sts の条件が満たされなかった場合のジャンプ先
- rai7,rai8,command,control,deprecated

# set_kataki_oya,人物ID:PersonId,仇人物ID:PersonId,親人物ID:PersonId
指定の人物IDに親の仇IDを設定します。
- rai7,command

# set_kataki_yome,人物ID:PersonId,仇人物ID:PersonId,嫁人物ID:PersonId
指定の人物IDに嫁、夫の仇IDを設定します。
- rai7,command

# set_kataki_kodomo,人物ID:PersonId,仇人物ID:PersonId,子人物ID:PersonId
指定の人物IDに子の仇IDを設定します。
- rai7,command

# get_kataki_oya,人物ID:PersonId
- rai7,command

# chk_wsts,人物ID:PersonId
指定人物IDの妊娠、初膜によって処理を分岐します。

case_normal   : ノーマル
case_botebara : 妊娠6か月以上
case_hatumaku : 初膜あり
- rai7,rai8,command,control

# case_normal
chk_wsts のノーマル時のジャンプ先
- rai7,rai8,command,control

# case_botebara
chk_wsts のボテ腹時のジャンプ先
- rai7,rai8,command,control

# case_hatumaku
chk_wsts の初膜時のジャンプ先
- rai7,rai8,command,control

# add_money,覇王ID:RulerId,追加資金額:Int
指定覇王IDに資金を追加します。
- rai7,command

# add_money,覇王ID:RulerId,追加資金額:Int
指定覇王IDに資金を追加します。
- rai8,command,deprecated

# add_urami,恨む覇王ID:RulerId,恨まれる覇王ID:RulerId,恨み値:Int
指定覇王IDに恨まれる覇王IDの恨み値を設定します。
恨み値範囲:10-400
- rai7,command

# set_capital,惑星ID:PlanetId,覇王ID:RulerId
指定覇王の首都を設定します。
- rai7,command

# get_capwid,覇王ID:RulerId
指定覇王の首都惑星IDを pval に設定します。
- rai7,rai8,command

# get_ncwid,覇王ID:RulerId
指定覇王の首都以外の惑星IDをランダムに pval に設定します。
- rai7,command

# set_yaku,人物ID:PersonId,役職ID:JobId
指定の人物に担当をセットします。
各担当の配属最大数を超えてセットはできません。
艦隊司令には指定できません。
- rai7,rai8,command

# add_kunkou,人物ID:PersonId,追加勲功値:Int
指定の人物に勲功を追加します。
マイナスを指定して勲功を下げることも可能。
アップできる勲功の最大は11000まで。
勲功を追加すると功績値も同じ値に設定されます。(ただし功績値が勲功より低い場合)
- rai7,rai8,command

# down_pkun,人物ID:PersonId,減少勲功値:Int
指定の人物に勲功を追加します。
- rai7,rai8,command

# up_pkun1,人物ID:PersonId
指定の人物を1階級昇格させます。
- rai7,rai8,command

# up_pkun2,人物ID:PersonId
指定の人物を2階級昇格させます。
- rai7,rai8,command

# point_up,p1:PersonId,p2:Int
- rai7,rai8,command

# ploy_down,覇王ID:RulerId,モード:Choice|0:信義派|1:武闘派,低下値:Int
指定覇王の信義派または武闘派の忠誠を低下させます。
低下値は指定の値を最大とし、0から低下値の範囲で忠誠が低下します。

モード
  0: 信義派
  1: 武闘派
- rai7,command

# ploy_up,覇王ID:RulerId,モード:Choice|0:信義派|1:武闘派,増加値:Int
指定覇王の信義派または武闘派の忠誠をアップさせます。
増加値は指定の値を最大とし、0から増加値の範囲で忠誠がアップします。

モード
  0: 信義派
  1: 武闘派
- rai7,command

# up_ploy,覇王ID:RulerId,モード:Choice|0:全武将|1:武闘派|2:信義派,増加値:Int
指定覇王の全武将、武闘派、信義派の忠誠をアップさせます。

モード
  0: 全武将
  1: 武闘派
  2: 信義派
- rai7,rai8,command

# dwn_ploy,覇王ID:RulerId,モード:Choice|0:全武将|1:武闘派|2:信義派,低下値:Int
指定覇王の全武将、武闘派、信義派の忠誠を低下させます。

モード
  0: 全武将
  1: 武闘派
  2: 信義派
- rai7,rai8,command


# set_union,盟主覇王ID:RulerId,覇王ID1:RulerId,覇王ID2:RulerId,覇王ID3:RulerId,覇王ID4:RulerId
新規で同盟を結成します。
参加覇王は4名まで。
覇王を指定しない場合は0を設定します。
- rai7,rai8,command

# add_union,盟主覇王ID:RulerId,追加覇王ID:RulerId,モード:Choice|0:通常同盟|1:従属同盟
すでにある同盟に追加覇王IDを参加させます。

モード
  0: 通常同盟
  1: 従属同盟
- rai7,command

# del_union,覇王ID:RulerId
指定の覇王を同盟から脱退させます。
指定覇王が盟主の場合は、同盟自体が解消します。
- rai7,rai8,command

# chk_exeplayer,人物ID1:PersonId,人物ID2:PersonId,人物ID3:PersonId,人物ID4:PersonId
プレイヤーが人物IDに合致する場合は chk_exeplayer_then へ処理がジャンプします。
そうでない場合は chk_exeplayer_else へ処理がジャンプします。
- rai7,rai8,command,control

# chk_exeplayer_then
chk_exeplayer の条件が満たされた場合のジャンプ先
- rai7,rai8,command,control

# chk_exeplayer_else
chk_exeplayer の条件が満たされなかった場合のジャンプ先
- rai7,rai8,command,control

# get_habatu2,派閥長ID:FactionId
指定派閥のナンバー2の人物IDをランダムで pval に設定します。
- rai7,rai8,command

# get_tyokan,覇王ID:RulerId,役職ID:JobId
指定覇王の担当IDに該当する中で階級の高い人物IDを pval に設定します。
- rai7,rai8,command

# get_habatumem,派閥長ID:FactionId,除外人物ID:PersonId
指定派閥のメンバーの人物IDをランダムで pval に設定します。
除外人物に指定されている人物IDはヒットしません。
- rai7,rai8,command

# get_prmpsn,覇王ID:RulerId,ステータスID:PersonStatusId
指定覇王の部下で指定ステータスが最も高い人物IDを pval に設定します。
- rai7,rai8,command

# dokuritu,派閥長ID:FactionId,惑星ID:PlanetId,モード:Choice|0:放浪旗揚げ|1:国家分裂
指定の派閥を指定惑星で独立させます。

モード
  0: 放浪旗揚げ
  1: 国家分裂
- rai7,command

# set_winfo,惑星ID:PlanetId,覇王ID:RulerId,人口:Int,資金:Int,民忠:Int,陸戦数:Int,X:Int,Y:Int
指定惑星の惑星情報を設定します。
設定しないパラメータは0を指定。
- rai7,command

# upd_winfo,惑星ID:PlanetId,覇王ID:RulerId,人口:Int,資金:Int,民忠:Int,陸戦数:Int,X:Int,Y:Int
指定惑星の惑星情報を更新します。
更新しないパラメータは0を指定。

set_winfo と異なり、現在の値からの差分値（相対値）を指定します。
- rai7,command

# set_seron,覇王ID:RulerId,世論値:Int
指定覇王の世論を設定します。
- rai7,command

# put_wakusei,惑星ID:PlanetId
非表示の惑星をHEX上に表示します。
事前に表示位置が出現可能な位置であるか if_cwpos で確認する必要あり。
- rai7,command

# put_wakusei,惑星ID:PlanetId,人物ID:PersonId
- rai8,command

# set_wjinko,惑星ID:PlanetId,増加人口:Int
指定の惑星に人口を追加します。
- rai7,command

# set_hexmap,X:Int,Y:Int,種別ID:Choice|0:フリーエリア|1:小惑星|2:ブラックホール|3:恒星|4:航行不可
HEXマップにアイコンを設定します。

種別ID
  0: フリーエリア
  1: 小惑星
  2: ブラックホール
  3: 恒星
  4: 航行不可
- rai7,command

# get_kunpsn,覇王ID:RulerId
指定覇王の部下で階級がもっとも高い人物IDを pval に設定します。
ただし王族はのぞく。
- rai7,command

# get_kunpsn,覇王ID:RulerId,除外人物ID:PersonId
指定覇王の部下で階級がもっとも高い人物IDを pval に設定します。
ただし王族はのぞく。
除外人物IDを設定すると、その人物は検索対象外になります。未設定の場合は0を指定。
- rai8,command

# get_kunpsn2,覇王ID:RulerId,除外人物ID:PersonId
指定の覇王IDの部下で、もっとも階級の高いものを pval に設定します。
ただし王族は除く。
除外人物IDを設定すると、その人物は検索対象外になります。未設定の場合は0を指定。
- rai7,command

# get_rndpsn,覇王ID:RulerId
指定覇王の部下で分5位以下の人物IDをランダムで pval に設定します。
- rai7,command

# mov_wid,惑星ID:PlanetId,覇王ID:RulerId
指定の惑星を指定の覇王IDの占領下に設定します。
- rai7,rai8,command

# chg_face,人物ID:PersonId,タイプ:Choice|0:ノーマル|1:mNNNNb.jpg|2:mNNNNc.jpg|3:mNNNNd.jpg|4:mNNNNe.jpg|5:mNNNNf.jpg|6:mNNNNg.jpg
指定人物の顔グラCGを変更します。

タイプ
  0: ノーマル
  1: mNNNNb.jpg
  2: mNNNNc.jpg
  3: mNNNNd.jpg
  4: mNNNNe.jpg
  5: mNNNNf.jpg
  6: mNNNNg.jpg
- rai7,command

# set_fort,要塞司令ID:PersonId,X:Int,Y:Int,モード:Choice|0:建設中|1:完成,要塞名:Speech
指定の位置に要塞を建設します。
事前に建設可能な位置であるか確認が必要。

モード
  0:建設中
  1:完成
- rai7,command

# cls_kantai,覇王ID:PersonId
指定覇王の艦隊を全てクリアします。
- rai7,command

# rel_weapon,覇王ID:PersonId
新型兵器のリリースを行う。
- rai7,command

# add_sdev,覇王ID:RulerId,タイプ:Choice|0:攻撃|1:対空対空|2:防御|3:掃海|4:建設|5:機動|6:艦載機攻撃|7:艦載機防御|8:陸戦攻撃|9:陸戦防御,増加値:Int
指定覇王の技術をアップします。

タイプ
  0:攻撃
  1:対空対空
  2:防御
  3:掃海
  4:建設
  5:機動
  6:艦載機攻撃
  7:艦載機防御
  8:陸戦攻撃
  9:陸戦防御
- rai7,command

# chg_pname,人物ID:PersonId,名前:Speech
人物に別名をセットします。
- rai7,rai8,command

# upd_sts,人物ID:PersonId,ステータスID:PersonStatusId,値:ReferPrevStatusId
ステータスIDに対応する値を変更します。
- rai7,command

# get_item,覇王ID:RulerId,装備人物ID:PersonId?,アイテム名:ItemName
指定覇王にアイテムを追加します。
アイテムが装備型の場合は、装備人物IDに装備されます。
すでに配布済のアイテムは追加できません。
- rai7,command

# for,繰り返し回数:Max,フラグNO:EveFlgNo,カウンタの開始値:Int
一般的なfor nextと使い方だいたい同じです。
フラグNOは eveflg の番号を指定します。この変数に繰り返しのカウンタが入ります。
繰り返し回数は数値以外に PMAX(人物最大値) SMAX(惑星最大値)が指定できます。
forの入れ子はできません。

例）
//繰り返し回数10回,eveflg:99番を使用,開始値 人物ＩＤ1から
for,10,99,1
    //忠誠が50以下の人物
    chk_sts,eveflg099,1,<50
    chk_sts_then
        zin_reg_msg,忠誠は50以下です,0
        continue
    chk_sts_else
        zin_reg_msg,忠誠は50以上です,0
        //continueをbreakにするとループ終了
        continue
next
- rai7,rai8,command,control
# continue
for ループを次に進めます。
- rai7,rai8,command,control
# break
for ループを終了します。
- rai7,rai8,command,control
# next
for ループの末尾を示します。
- rai7,rai8,command,control

# psn_putsex,オルドタイプ:Choice|1:黒子オルド|2:人物ごとのオルド,人物ID:PersonId,CGタイプ:Choice|1:通常オルド|2:拷問オルド
以下で黒子のオルドムービーを表示します。
psn_putsex,1,0,1
- rai7,command

# zin_old_init
オルドイベント初期化処理
- rai7,rai8,command


# zin_old_img_pid,人物ID:PersonId,表示モード:Choice|0:瞬間表示|1:フェードイン表示
人物IDに対応したオルドCGを表示します。
オルドCGがない場合は汎用画像が表示されます。

表示モード
  0:瞬間表示
  1:フェードイン表示
- rai7,rai8,command

# zin_old_img_str,オルドファイル名:File,dpmID:File,表示モード:Choice|0:瞬間表示|1:フェードイン表示
オルドCGファイル名を直接指定してオルドCGを表示します。

dpmID
  pac0.dpmの番号。pacの次に続く番号
  pacフォルダにファイルがある場合は0を指定

表示モード
  0:瞬間表示
  1:フェードイン表示
- rai7,rai8,command

# zin_old_msg,メッセージ:Speech,人物ID:PersonId
オルドイベントで台詞位置にメッセージを表示します。
人物IDで指定した顔グラが表示されます。
人物IDに 0 を指定するとナレーション表示になります。
※ナレーションはzin_old_qnarコマンドの使用をお奨めします。
- rai7,rai8,command

# zin_old_qnar,メッセージ:Speech?,人物ID:PersonId?
オルド画面専用のナレーションを表示します。
中央黒帯背景。
- rai7,rai8,command

# zin_old_fadeout,モード:Choice|0:黒フェードアウト|1:白フェードアウト,フェードタイム:Int
オルド画面をフェードアウトします。

モード
  0:黒フェードアウト
  1:白フェードアウト

フェードタイム
  5から30を指定
- rai7,rai8,command
 
# zin_old_exit
- rai7,rai8,command
 
# zin_flush,人物ID:PersonId
オルド画面を３回フラッシュします。
人物IDはフラッシュ後に表示するオルドCGの人物IDを指定します。
オルドCGと人物IDの関連がない場合は zin_flush_file を使用します。
- rai7,rai8,command

# zin_flush_file,オルドファイル名:File,dpmID:Any
オルド画面を３回フラッシュします。
オルドファイル名はフラッシュ後に表示するオルドCGのファイル名を指定します。
dpmIDはオルドファイルが格納されているdpmIDを指定します。pacフォルダにファイルがある場合は0を指定します。
- rai7,command

# zin_flush_file_one,オルドファイル名:File,dpmID:Any
オルド画面を一度だけフラッシュします。
オルドファイル名はフラッシュ後に表示するオルドCGのファイル名を指定します。
dpmIDはオルドファイルが格納されているdpmIDを指定します。pacフォルダにファイルがある場合は0を指定します。
- rai7,command

# zin_flush_one,人物ID:PersonId
オルド画面を一度だけフラッシュします。
人物IDはフラッシュ後に表示するオルドCGの人物IDを指定します。
オルドCGと人物IDの関連がない場合は zin_flush_file_one を使用します。
- rai7,rai8,command

# zin_reg_button,ボタンID:Choice|0:OKボタン|3:2択|4:外交3択|5:捕虜3択|6:捕虜2択|9:オルド 強引にやる 2択|10:営業2択|11:外交2択|12:バイオノイド購入選択|13:技術購入2択|16:オルド画面用OKボタン
イベントにボタン表示する

get_respで選択結果を取得する
  ret_yes: 承諾を選んだ場合
  ret_no: 拒否を選んだ場合

ボタンID
  0:OKボタン
  3:2択
  4:外交3択
  5:捕虜3択
  6:捕虜2択
  9:オルド 強引にやる　2択
  10:営業2択
  11:外交2択
  12:バイオノイド購入選択
  13:技術購入2択
  16:オルド画面用OKボタン
- rai7,command
# ret_yes
zin_reg_button のジャンプ先
- rai7,command,control
# ret_no
zin_reg_button のジャンプ先
- rai7,command,control

# zin_reg_button,ボタンID:Choice|0:OKボタン|3:2択|4:外交3択|5:捕虜3択|6:捕虜2択|9:オルド 強引にやる 2択|10:営業2択|11:外交2択|12:バイオノイド購入選択|13:技術購入2択|16:オルド画面用OKボタン|19:オルド画面用OKボタン(八星帝)
イベントにボタン表示する

ボタンID
  0:OKボタン
  3:2択
  4:外交3択
  5:捕虜3択
  6:捕虜2択
  9:オルド 強引にやる　2択
  10:営業2択
  11:外交2択
  12:バイオノイド購入選択
  13:技術購入2択
  16:オルド画面用OKボタン
  16:オルド画面用OKボタン
  19:オルド画面用OKボタン(八星帝)
- rai8,command

# zin_reg_2sel,選択肢1:Speech,選択肢2:Speech
2択のダイアログを表示します。
選択結果は get_resp コマンドで取得します。
  ret_sel1: 選択肢1を選んだ場合
  ret_sel2: 選択肢2を選んだ場合
- rai7,rai8,command,control

# zin_reg_3sel,選択肢1:Speech,選択肢2:Speech,選択肢3:Speech
3択のダイアログを表示します。
選択結果は get_resp コマンドで取得します。
  ret_sel1: 選択肢1を選んだ場合
  ret_sel2: 選択肢2を選んだ場合
  ret_sel3: 選択肢3を選んだ場合
- rai7,rai8,command,control

# zin_reg_init
イベント画面を初期化します。
- rai7,rai8,command

# zin_reg_exit
イベント画面を終了します。
- rai7,rai8,command

# zin_reg_event_mode,タイトル文字:Speech,人物ID1:PersonId,人物ID2:PersonId,zinファイル名:File?,gzinファイル名:File,p6:Any
- rai7,command
 
# zin_reg_event,タイトル文字:Speech?,人物ID1:PersonId?,人物ID2:PersonId?,zinファイル名:File?,gzinファイル名:File
イベント画面にイベントCG（gzin画像）を表示します。
人物ID1は左、人物ID2は右に表示されます。
- rai7,rai8,command

# zin_reg_title,メッセージ:Speech?
イベント画面の上部タイトルエリアにメッセージを表示します。
- rai7,rai8,command

# zin_reg_msg,メッセージ:Speech,台詞位置:Choice|0:左側|1:右側
台詞位置にメッセージ（吹き出し）を表示します。

台詞位置
  0:左側
  1:右側
- rai7,rai8,command

# zin_regs_msg,男台詞:Speech,女台詞:Speech,台詞位置:Choice|0:左側|1:右側
zin_reg_event で指定された人物IDの性別を判定して合致した台詞を表示します。

台詞位置
  0:左側
  1:右側
- rai7,rai8,command


# zin_reg_nar,メッセージ:Speech
ナレーションメッセージを表示します。
背景指定はありませんので、通常 bl_fadeout の後に記述します。
- rai7,rai8,command

# zin_reg_qnar,メッセージ:Speech?,p1:Any?
背景中央黒帯のナレーションメッセージを表示します。
- rai7,rai8,command

# zin_reg_face,人物ID:PersonId
イベント画面の中央に顔CGを表示します。
人物IDに0を指定すると、非表示になります。
- rai7,rai8,command

# zin_reg_face_r,人物ID:PersonId
左の顔グラを指定の人物に変更します。
人物IDに0を指定すると、非表示になります。
- rai7,rai8,command

# zin_reg_face_0,人物ID:PersonId?
左の顔グラを指定の人物に変更します。
人物IDに0を指定すると、非表示になります。
- rai7,rai8,command

# zin_reg_face_l,人物ID:PersonId
右の顔グラを指定の人物に変更します。
人物IDに0を指定すると、非表示になります。
- rai7,rai8,command

# zin_reg_face_1,人物ID:PersonId
右の顔グラを指定の人物に変更します。
人物IDに0を指定すると、非表示になります。
- rai7,rai8,command

# zin_dialog,メッセージ:Speech
ダイアログを表示します。
- rai7,command

# bl_fadein,gzinファイル名:File
ブラック背景からのフェードイン
- rai7,rai8,command

# bl_fadeout
ブラックフェードアウト
- rai7,rai8,command

# wt_fadeout
ホワイトフェードアウト
- rai7,rai8,command

# set_pmov,人物ID:PersonId,覇王ID:RulerId
指定人物を指定覇王に移籍させます。
勲功（階級）は低下します。
忠誠は元の忠誠値が反転します。（忠誠が高い→低くなる、低い→高くなる）
- rai7,rai8,command

# select_pid,人物ID:PersonId
人物IDごとに処理を分岐します。
例）
pid1の人物IDが100だった場合、case_100へ処理が移動する
case_100の処理が終了した場合、最後にgotoコマンドがないと、そのまま以下のコマンドを実行してしまうので注意。

select_pid,pid1
case_100
  zin_reg_msg,おい、貴様,1
  zin_reg_msg,うん？　誰だお前は？,0
  goto,*label1
case_200
  zin_reg_msg,貴様、何者だ！！,0
  zin_reg_msg,フフ、そんな腕で勝てるのか？,1
  goto,*label1
case_300
  zin_reg_msg,おい、貴様,1
  zin_reg_msg,うん？　誰だお前は？,0
  goto,*label1
default
  zin_reg_msg,おい、貴様,1
  zin_reg_msg,うん？　誰だお前は？,0

*label1
- rai7,command,control

# default
select_pid のジャンプ先
- rai7,command,control

# dec_sinmitu,人物ID:PersonId,減少値:Int
人物IDの親密値を減少値だけ減らす。
- rai7,rai8,command

# add_sinmitu,人物ID:PersonId,増加値:Int
人物IDの親密値を増加値だけ増やす。
- rai7,rai8,command

# set_sinmitu,人物ID:PersonId,親密値:Int
人物IDの親密値を指定の値に設定します。
- rai7,rai8,command

# set_non_virgin,人物ID:PersonId
人物IDを初膜なしに設定します。
- rai7,rai8,command

# set_virgin,人物ID:PersonId
人物IDを初膜ありに設定します。
- rai7,rai8,command

# chg_player,to人物ID:PersonId,from人物ID:PersonId
to人物IDにプレイヤーを変更します。
fromが設定されている（0以外）場合はfrom人物ID=playerの場合のみプレイヤーが入れ替わります。
- rai7,rai8,command

# sel_player,人物ID:PersonId
- rai7,command

# ouzoku_tuiho,p1:PersonId,p2:PersonId,p3:PersonId,p4:PersonId,p5:PersonId,p6:PersonId
- rai7,command

# set_moriyaku,人物ID:PersonId
- rai7,command

# set_kan_sirei,覇王ID:RulerId,人物ID:PersonId
- rai7,command

# chk_pterm,人物ID:PersonId
- rai7,command,control
# case_pterm0
- rai7,command,control

# case_pterm1
- rai7,command,control

# case_pterm2
- rai7,command,control

# case_pterm3
- rai7,command,control

# bgm_se,効果音ID:Any
効果音を鳴らします。
- rai7,rai8,command

# bgm_chg,BGMID:Any
BGMをループ再生します。
- rai7,rai8,command

# bgm_chg_nl,BGMID:Any
BGMは再生します。ループはしません。
- rai7,command

# bgm_stop
BGMを停止します。
- rai7,rai8,command

# zin_seloop,効果音ID:Any,モード:Any
効果音をループ再生させます。
zin_seloop_stopコマンドで、ループが終了します。
- rai7,command,deprecated

# zin_seloop_stop
効果音のループ再生を終了します。
- rai7,command,deprecated

# sento,覇王ID:RulerId,惑星ID:PlanetId
指定覇王が指定惑星に遷都します。
- rai7,rai8,command

# set_dead,死亡人物ID:PersonId,仇人物ID:PersonId
人物を死亡させます。
仇人物IDを指定すると、死亡人物IDの家族に仇が設定されます。
仇人物IDを指定しない場合は0を指定します。

人物状態ステータスを変更する場合は、upd_stsを使用せずに、なるべく専用コマンドの利用をお奨めします。
upd_stsは直接数値を変更するだけですが、このコマンドでは人物の状態変更以外に役職解除や保有アイテムの処理等も合わせて実行します。
- rai7,rai8,command

# set_pkon,人物ID1:PersonId,人物ID2:PersonId
人物ID1と人物ID2を結婚させます。
- rai7,rai8,command

# set_rikon,人物ID:PersonId
人物IDが結婚している場合、離婚状態に設定します。
- rai7,rai8,command

# set_hourou,人物ID:PersonId
人物IDを放浪状態（状態ステータス:5）にします。

人物状態ステータスを変更する場合は、upd_stsを使用せずに、なるべく
専用コマンドの利用をお奨めします。
upd_stsは直接数値を変更するだけですが、このコマンドでは人物の状態変更以外に
役職解除や保有アイテムの処理等も合わせて実行します。
- rai7,rai8,command

# set_horyo,人物ID:PersonId,覇王ID:RulerId
指定の人物が指定覇王の捕虜になります。
- rai7,command
  
# set_horyo,人物ID:PersonId,覇王ID:RulerId,罪状ID:Any?
指定の人物が指定覇王の捕虜になります。
- rai8,command

# set_horyo_sikan,p1:PersonId,p2:Any?,p3:Any?,p4:Any?,p5:Honor
- rai8,command

# set_sikan,人物ID:PersonId,覇王ID:RulerId
指定の人物が指定覇王に仕官します。
- rai7,rai8,command
 
# set_sikan2,人物ID:PersonId,覇王ID:RulerId,勲功:Honor
指定の人物が指定の勲功で指定覇王に仕官します。
- rai7,rai8,command

# chg_haou,覇王ID:RulerId,人物ID:PersonId
指定の人物が指定陣営の覇王になります。
変更前の覇王は死亡します。
- rai7,rai8,command

# set_netorare,p1:PersonId,p2:Any?,p3:PersonId
- rai8,command

# set_birth,子供ID:PersonId,父ID:PersonId,母ID:PersonId,子の勲功:Honor
子供が生まれる。
- rai7,rai8,command

# set_ninsin,女人物ID:PersonId,種父ID:PersonId
指定の女人物が妊娠します。
- rai7,rai8,command

# make_name,人物ID:PersonId
- rai7,rai8,command

# set_flagship,人物ID:PersonId,旗艦ID:ShipId
指定人物の旗艦を設定します。
- rai7,command

# get_puu,覇王ID:RulerId,人数:Int
指定の覇王に在野を仕官させます。
指定した人数を一度に仕官できますが、在野が存在しない場合は仕官できません。
- rai7,rai8,command

# rnd,乱数値:Int
乱数値に基づく乱数を発生させ、発生した値ごとに処理を分岐します。
乱数値4を指定した場合、0から3の乱数が発生します。
その場合は、case0/1/2/3のcase文が必要です。
乱数値には2以上を指定してください。
- rai7,rai8,command,control

# get_resp
コマンドの結果を取得して、ret_sel1、ret_sel2、ret_sel3にジャンプします。
- rai7,rai8,command,control

# ret_sel1
get_resp コマンドのジャンプ先
- rai7,rai8,command,control

# ret_sel2
get_resp コマンドのジャンプ先
- rai7,rai8,command,control

# ret_sel3
get_resp コマンドの場合のジャンプ先
- rai7,rai8,command,control

# end
イベントを終了します。
- rai7,rai8,command,control

# case_[0-9]+
分岐のジャンプ先
- rai7,rai8,command,control,regex

# case[0-9]+
rnd コマンドのジャンプ先
- rai7,command,control,regex

# sel_plist,p1:Any,p2:RulerId,p3:Any,p4:Any,p5:Speech
- rai7,command
 
# sel_plist,p1:Any,p2:RulerId,p3:Speech
- rai8,command
 
# sel_plist2,モード:Choice|2:諜報潜入|3:救出捕虜選択|4:捕虜選択|5:継承覇王選択|7:プレイヤー選択|9:修行部下選択|13:政略結婚(女)|14:政略結婚(男)|16:独身の妾|33:覇王以外の王族|35:妃/姫/王子選択|プレイヤー覇王選択|38:拉致王族選択|39:部下選択|40:部下選択|41:育成対象,覇王ID:RulerId,メッセージ:Speech,拡張1:Any,年齢制限:Age
覇王IDに所属する人物選択リストを表示します。
選択した人物IDがpvalに設定されます。

一度表示した選択画面をキャンセルすることはできません。
表示前に対象となる人物が存在するかのチェックが必要です。

拡張1:
  指定したモードによってパラメータの意味が異なります。
  通常は0を設定してください。
年齢制限:
  1以上を設定すると、指定年齢より下の人物を除外

モード
  2:諜報潜入
  3:救出捕虜選択
  4:捕虜選択
  5:継承覇王選択 女は除外
  7:プレイヤー選択 女は除外
  9:修行部下選択
      拡張1  1:戦術80以上を除外  2:格闘80以上を除外
  13:政略結婚 結婚させる独身女王族
  14:政略結婚 婚姻相手の独身男王族
      拡張1  家族の人物ID
  16:独身の妾
  33:覇王以外の王族
  35:妃/姫/王子選択
      拡張1  12100:姫   12200:妃  12300:王子
  36:プレイヤー覇王選択 女は除外
  38:拉致王族選択
  39:部下選択　覇王含む
      拡張1  10:階級9000以上は除く
  40:部下選択　覇王除く
      拡張1  除外人物ID
  41:育成対象(王子、姫、孫)
- rai8,command

# sel_mplist,p1:Any,覇王ID:RulerId,メッセージ:Speech,p4:Int
- rai8,command
# get_mplist,p1:Int
- rai8,command

# opencg,人物ID:PersonId
指定人物のCGモードを解放します。
- rai8,command

# btl_kakuto,人物ID1:PersonId,人物ID2:PersonId,モードA:Choice|0:負けたら死亡|1:負けたら死亡,背景画像:Choice|0:暗殺背景|1:背景なし|3:武闘大会,モードB:Choice|0:体力引き継ぎなし|1:体力引き継ぎあり
人物ID1と人物ID2で格闘戦を開始します。
このコマンドはインデントが有効です。

case_winA: 人物ID1が勝利した場合の分岐
case_winB: 人物ID2が勝利した場合の分岐

モードA
  0:負けても死亡しない
  1:死ぬ
背景画像
  0:暗殺背景
  1:部下同士 背景なし
  3:武闘大会
モードB(暗殺者の体力引き継ぎ)
  0:引き継ぎなし
  1:引き継ぎあり
- rai8,command,control

# case_winA
btl_kakuto で人物ID1が勝利した場合のジャンプ先
- rai8,command,control

# case_winB
btl_kakuto で人物ID2が勝利した場合のジャンプ先
- rai8,command,control

# kakuto_team,a1:PersonId,a2:PersonId,a3:PersonId,b1:PersonId,b2:PersonId,b3:PersonId,p7:Any,gzinファイル名:File
- rai8,command,control

# surrend,覇王ID1:RulerId,覇王ID2:RulerId,モード:Choice|0:部下|1:捕虜|2:追放|3:死刑
覇王ID2を覇王ID1に併合します。

モード(併合される王族の処分)
  1:部下
  2:捕虜
  3:追放
  4:死刑
- rai8,command

# z8_old_dvc_play,wavファイル名:File,モード:Choice|off:ループなし|on:ループあり
ボイスをダイレクト再生します。
停止するには z8_old_bgv_stop を実行します。

モード
  off:ループなし
  on:あり
- rai8,command

# wswap,主星ID1:PrimaryPlanetId,主星ID2:PrimaryPlanetId
異なる覇王間で主星ID1と主星ID2を交換します。
- rai8,command

# eveflg_countup,フラグNO:EveFlgNo,カウント終了値:Int
指定のイベントフラグIDを毎月＋１します。
カウント終了値になるとカウントを終了します。
- rai8,command

# eveflg_countcls,フラグNO:EveFlgNo
eveflg_countupで登録したイベントフラグのカウントを強制終了します。
- rai8,command

# z8_wait,1/100秒数:Int
指定時間だけ処理を停止します。
- rai8,command

# z8_win_fadein,画像ID:Choice|wb01:宇宙/明|wb02:宇宙/暗|wb03:訪問フェーズ背景|wb04:恒星系背景|wb05:オルド背景|wb06:オープニング背景|wb07:ハナヒゲ遊郭背景
全画面フェードイン

画像ID
  wb01: 宇宙/明
  wb02: 宇宙/暗
  wb03: 訪問フェーズ背景
  wb04: 恒星系背景
  wb05: オルド背景
  wb06: オープニング背景
  wb07: ハナヒゲ遊郭背景
- rai8,command

# set_tgthao,覇王ID1:RulerId,覇王ID2:RulerId
覇王ID1に覇王ID2の惑星を攻撃目標として設定します。
- rai8,command

# set_tm,恒星系ID:StarId,覇王ID:RulerId
指定の恒星系IDに覇王IDのターゲットマーカーを設置します。
- rai8,command

# r8_dialog,メッセージ:Speech
ダイアログを表示します。
- rai8,command

# r8_dialog2,メッセージ:Speech
- rai8,command,control
# case_no
- rai8,command,control
# case_yes
- rai8,command,control

# set_fc_filter,左上:Choice|0:off|1:on,左下:Choice|0:off|1:on,右上:Choice|0:off|1:on,右下:Choice|0:off|1:on
顔フィルターを設定します。1(on)にすると以降の処理で顔グラに各種フィルターがかかります。
現在は牢屋フィルターのみです。
元に戻す場合は0(off)に設定してください。

牢屋フィルター
  0:off
  1:on
- rai8,command

# back_kasi,人物ID:PersonId,覇王ID:RulerId
set_kasi で仮死状態にした人物IDを仕官中に設定します。
覇王IDが指定されている場合、指定覇王に仕官する状態に設定されます。
覇王IDが0の場合、元の覇王に仕官します。
- rai8,command

# plt_apr,覇王ID:RulerId,惑星ID:PlanetId
非表示状態の惑星を覇王IDの惑星として出現させます。
- rai8,command

# set_pid,pid1:PersonId,pid2:PersonId,pid3:PersonId,pid4:PersonId
pid1,pid2,pid3,pid4に人物IDを設定します。
- rai8,command,if

# chg_face,人物ID:PersonId,変更記号:Choice|a|c|d|e|f|g|0
人物IDの顔グラを変更します。
cからgまで５種類の顔グラを事前に用意して、切り替えることができます。
元に戻す場合は変更記号にaを指定します。

変更記号
  a:mNNNN.jpg
  c:mNNNNc.jpg
  d:mNNNNd.jpg
  e:mNNNNe.jpg
  f:mNNNNf.jpg
  g:mNNNNg.jpg
- rai8,command

# add_mny,覇王ID:RulerId,追加資金:Int
指定覇王に資金を追加します。
追加資金にマイナスを設定した場合は減少します。
- rai8,command

# add_mtl,覇王ID:RulerId,追加資材:Int
指定覇王に資材を追加します。
追加資材にマイナスを設定した場合は減少します。
- rai8,command

# add_ene,覇王ID:RulerId,追加エネルギー:Int
指定覇王にエネルギーを追加します。
追加エネルギーにマイナスを設定した場合は減少します。
- rai8,command

# add_wat,覇王ID:RulerId,追加水:Int
指定覇王に水を追加します。
追加水にマイナスを設定した場合は減少します。
- rai8,command

# set_sinrai,覇王ID1:RulerId,覇王ID2:RulerId,信頼値:Int
覇王ID1と覇王ID2の信頼値を同じ値に設定します。
- rai8,command

# cul_sinrai,覇王ID1:RulerId,覇王ID2:RulerId,増減値:Int
覇王ID1における覇王ID2の信頼値を増減します。
- rai8,command

# get_item,覇王ID:RulerId,アイテム名:ItemName,ランク:Choice|S1|S2|S3|S4|S5|SS|T1|T2|T3|T4|T5|T10
指定覇王にアイテムを設定します。
- rai8,command

# item_img
- rai8,command

# add_senkan,覇王ID:RulerId,艦種ID:ShipId
指定覇王に艦艇を追加します。
- rai8,command

# mov_yamato
- rai8,command

# chk_old_type,人物ID:PersonId,p1:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし,p2:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし,p3:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし,p4:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし,p5:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし,p6:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし,p7:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし,p8:Choice?|体位あり|体位なし|胸サイズあり|胸サイズなし|親密あり|親密なし|性格あり|性格なし|年齢あり|年齢なし|結婚あり|結婚なし|言葉あり|言葉なし|回数あり|回数なし
人物IDのオルドステータスに特化した状態にマッチしたcase_otに分岐します。
各ステータスを判定したい条件のみ「XXXXあり」と指定します。
「XXXXあり」と指定した条件のみcase_otに追加文字列が追加されます。
case_ot_break、case_ot_defaultは一般言語のswitch/case文のbreak/defaultと同等です。
処理の最後にcase_ot_endを記述してください。

体位: 体位あり/体位なし　追加文字列:正常位/バック/騎乗位
胸サイズ: 胸サイズあり/胸サイズなし　追加文字列:貧乳/標準/巨乳
親密: 親密あり/親密なし　追加文字列:普通/嫌悪/信頼
性格: 性格あり/性格なし　追加文字列:普通/欲情
年齢: 年齢あり/年齢なし　追加文字列:成人/ロリ/熟女
結婚: 結婚あり/結婚なし　追加文字列:独身/人妻
言葉: 言葉あり/言葉なし　追加文字列:言葉A/言葉B/言葉C/言葉D
回数: 回数あり/回数なし　追加文字列:N回目 （N:最大3回目まで）

例）性格ありと結婚ありを指定した場合のcase_otは
case_ot通常_普通_独身
case_ot通常_普通_人妻
case_ot通常_欲情_独身
case_ot通常_欲情_人妻
case_otボテ_普通_独身
case_otボテ_普通_人妻
case_otボテ_欲情_独身
case_otボテ_欲情_人妻
case_ot初膜_普通_独身
case_ot初膜_普通_人妻
case_ot初膜_欲情_独身
case_ot初膜_欲情_人妻
以上、12種類のcase_otが指定可能となり、マッチした条件に分岐します。
追加文字列はアンダーバーで区切ります。
デフォルトで通常、ボテ、初膜の３状態が追加されます。
case_otの追加文字列は「XXXXあり」と指定した条件の順番で文字が追加されますので注意してください。
なおすべてのcase_ot分岐を記載する必要はなく、必要な分岐のみ記述して、最後にcase_ot_defaultを記述すれば、該当しない条件はすべてcase_ot_defaultに分岐されます。

例）次のような省略した記載も可能です
chk_old_type,人物ID,性格あり,結婚あり,,,,,,
case_ot通常_普通_独身
	通常_普通_独身の処理
case_ot_break
case_ot_default
	条件未該当の処理
case_ot_end
- rai8,command,control

# case_ot[^_].+
chk_old_type のジャンプ先
- rai8,command,control,regex
# case_ot_default
chk_old_type のデフォルトジャンプ先
- rai8,command,control
# case_ot_break
chk_old_type の break 処理
- rai8,command,control
# case_ot_end
case_ot_break のジャンプ先
- rai8,command,control

# chk_pcustom,人物ID1:PersonId,人物ID2:PersonId,判定ステータス1:Choice|家族|派閥|階級|状態|体位|胸サイズ|親密|性格|年齢|年代|結婚|言葉|回数|絶頂,判定ステータス2:Choice?|家族|派閥|階級|状態|体位|胸サイズ|親密|性格|年齢|年代|結婚|言葉|回数|絶頂,判定ステータス3:Choice?|家族|派閥|階級|状態|体位|胸サイズ|親密|性格|年齢|年代|結婚|言葉|回数|絶頂,判定ステータス4:Choice?|家族|派閥|階級|状態|体位|胸サイズ|親密|性格|年齢|年代|結婚|言葉|回数|絶頂,判定ステータス5:Choice?|家族|派閥|階級|状態|体位|胸サイズ|親密|性格|年齢|年代|結婚|言葉|回数|絶頂,判定ステータス6:Choice?|家族|派閥|階級|状態|体位|胸サイズ|親密|性格|年齢|年代|結婚|言葉|回数|絶頂
人物ID1のステータスにマッチしたcase_pcに処理を分岐します。
判定したい条件を判定ステータスを記述します。
記述した判定ステータスによってcase_pcに追加文字列が追加されます。
追加文字列はアンダーバーで区切ります。
case_pcの追加文字列は判定ステータスに記述した順番で文字が追加されますので注意してください。
case_pc_break、case_pc_defaultは一般言語のswitch/case文のbreak/defaultと同等です。
処理の最後にcase_pc_endを記述してください。
判定ステータスに家族 派閥 階級を指定した場合は人物ID2との関係になります。
家族 派閥 階級以外を指定した場合は、人物ID2に0を指定してください。

このコマンドはインデントが有効です。chk_pcustomおよびcase_pcXXXXはインデントを揃える必要があります。

判定ステータス と 対応するcase_pcの追加文字列
  家族:　息子 娘 弟 妹 兄 姉 父 母 夫 妻 他人 本人　＞義理家族系も指定できるっぽい。全ては確認して無いが「義弟」は動作した。
  派閥:　異派閥 同派閥（※現在未使用）
  階級:　部下　上官　覇王　同級
  状態:　通常　ボテ　初膜　
  体位:　正常位　バック　騎乗位
  胸サイズ:　貧乳　標準　巨乳（※現在未使用）
  親密:　普通　嫌悪　信頼
  性格:　普通　痴女
  年齢:　成人　ロリ　熟女
  年代:　10代　20代　30代　40代　50代
  結婚:　独身　人妻　
  言葉:　言葉A　言葉B　言葉C　言葉D
  回数:　n回目　（n:最大3回目まで）
  絶頂:　絶頂n回 （n:最大10回まで）
- rai8,command,control

# case_pc_.+
chk_pcustom のジャンプ先
- rai8,command,control,regex
# case_pc_default
chk_pcustom のデフォルトジャンプ先
- rai8,command,control
# case_pc_break
chk_pcustom の break 処理
- rai8,command,control
# case_pc_end
case_pc_break のジャンプ先
- rai8,command,control

# union_new,盟主覇王ID:RulerId,参加覇王1:RulerId,モード1:Choice|0:通常同盟|1:従属同盟,参加覇王2:RulerId?,モード2:Choice?|0:通常同盟|1:従属同盟,参加覇王3:RulerId?,モード3:Choice?|0:通常同盟|1:従属同盟,参加覇王4:RulerId?,モード4:Choice?|0:通常同盟|1:従属同盟
同盟を新規作成します。
最大5カ国で同盟を作成できます。
盟主覇王と参加覇王1の設定は必須です。
5カ国に満たない同盟の場合は、参加覇王に0を指定してください。

モード:
  0:通常同盟
  1:従属同盟
- rai8,command

# switch_pid,人物ID:PersonId
人物IDごとに処理を分岐します。
case_pid_breakが実行されるとcase_pid_endへ処理が移動します。
case_pidに該当するIDがない場合はcase_pid_defaultへ処理が移動します。
case_pid_defaultは最後の分岐に記述してください。case_pid_default以降にあるcase_pidXXXは無視されます。

例）pid2に設定されている人物IDごとに処理を分岐します。
switch_pid,pid2
case_pid512
	処理A
	case_pid_break
case_pid16
	処理B
	case_pid_break
case_pid_default
	処理C
case_pid_end
- rai8,command,control

# case_pid[0-9]+
switch_pid のジャンプ先
- rai8,command,control
# case_pid_default
switch_pid のデフォルトジャンプ先
- rai8,command,control
# case_pid_break
switch_pid の break 処理
- rai8,command,control
# case_pid_end
case_pid_break のジャンプ先
- rai8,command,control

# z8_event,左上人物ID:PersonId,左下人物ID:PersonId,右上人物ID:PersonId,右下人物ID:PersonId,フェード:Choice|0:フェードあり|1:フェードなし,gzinファイル名:File,zinファイル名:File?,タイトル文字:Speech?
イベント画面を作成します。
最大4人の人物を表示できます。表示しない場合は0を指定してください。
zinファイル名、タイトル文字は省略できます。
gzinファイル名およびzinファイル名の拡張子(.jpg)は省略します。
フェード
  0:フェードなし
  1:フェードあり
- rai8,command

# z8_msg,表示位置:Choice|1:左上|2:左下|3:右上|4:右下,男台詞:Speech?,女台詞:Speech?
z8_event で表示したキャラに台詞を表示します。
表示位置で指定した人物IDが男の場合、男台詞が表示されます。
表示位置で指定した人物IDが女の場合、女台詞が表示されます。
女台詞は省略可能です。省略した場合は男台詞が表示されます。

表示位置
  1:左上
  2:左下
  3:右上
  4:右下
- rai8,command

# z8_w4msg,表示位置:Choice|1:左上|2:左下|3:右上|4:右下,台詞A:Speech,台詞B:Speech?,台詞C:Speech?,台詞D:Speech?
z8_event で表示したキャラに台詞を表示します。
表示位置で指定した人物IDの性格によって表示される台詞が決まります。
台詞B,台詞C,台詞Dは省略可能です。省略した場合は台詞Aが表示されます。

表示位置
  1:左上
  2:左下
  3:右上
  4:右下

台詞A: サル コアラ ゾウ ライオン
台詞B: コジカ タヌキ ヒツジ
台詞C: ペガサス チータ
台詞D: トラ オオカミ 黒ヒョウ
- rai8,command

# z8_qnar,wait:Choice|0:ウェイトなし|1:ウェイトあり,ナレーション文字:Speech
イベントサイズ640x480のナレーションを画面中央に表示します。
ウェイトありの場合、キー入力があるまで、次の処理に進みません。

waitモード
  0:ウェイトなし
  1:ウェイトあり
- rai8,command

# z8_zimg,zinファイル名:File
zin画像をイベント画面に表示します。

zinファイル名: zin画像のファイル名を指定します。拡張子(.jpg)は省略します。
- rai8,command

# z8_sel,選択肢1:Speech,選択肢2:Speech?,選択肢3:Speech?,選択肢4:Speech?,選択肢5:Speech?,選択肢6:Speech?,選択肢7:Speech?,選択肢8:Speech?,選択肢9:Speech?
選択メニューを表示します。
選択肢は最大9項目登録できます。
選択したメニューに該当する case_zs に処理が分岐します。
例）選択文字列1を選択した場合は case_zs1 に処理が移動します。
該当する case_zs が存在しない場合は case_zs_end に処理が移動します。
- rai8,command,control

# case_zs[1-9]
z8_sel のジャンプ先
- rai8,command,control,regex

# case_zs_break
z8_sel の break 処理
- rai8,command,control

# case_zs_end
case_zs_break のジャンプ先
- rai8,command,control

# z8_old_sel3,人物ID:PersonId,拒否:Choice?|拒否あり|拒否なし,状態:Choice?|状態あり|状態なし,年齢:Choice?|年齢あり|年齢なし,結婚:Choice?|結婚あり|結婚なし,言葉:Choice?|言葉あり|言葉なし,回数:Choice?|回数あり|回数なし
- rai8,command,control

# case_so.+
z8_old_sel3 のジャンプ先
- rai8,command,control,regex

# z8_face,表示位置:Choice|1:左上|2:左下|3:右上|4:右下,人物ID:PersonId|0:落ちる|-1:揺れ|-2:クリア
指定した人物IDに顔CGを変更します。

人物IDに0,-1,-2を指定できます。z8_eventで設定されている表示位置の人物IDに対して次の動作を指定できます。
   0: 顔CGが下へ落ちて非表示になります。
  -1: 顔CGが左右に揺れます。
  -2: 顔CGを非表示にします。

表示位置
  1:左上
  2:左下
  3:右上
  4:右下
- rai8,command

# upd_psts,人物ID:PersonId,ステータスID:PersonStatusId,値:ReferPrevStatusId
ステータスIDに対応する項目から指定値を加算します。
値にマイナスを指定した場合は減算になります。
- rai8,command

# set_psts,人物ID:PersonId,ステータスID:PersonStatusId,値:ReferPrevStatusId
ステータスIDに対応する項目を指定値にセットします。
- rai8,command

# get_skill,人物ID:PersonId,スキル名:SkillName
指定の人物にスキルを追加します。
人物のスキルは３つまで追加できますが、空きがない場合は追加されません。

スキル文字列: csvフォルダにあるsk_base.csvのスキル名を指定
- rai8,command

# z8_old_init,男人物ID:PersonId,女人物ID:PersonId,状況:Choice|和姦|強引|淫乱|捕虜|従順|媚薬|弱み,gzinファイル名:File
オルド処理を初期化します。
状況の値によって台詞が変化します。
z8_old_touch を使用する場合、この初期化処理が必要です。

状況:
  和姦
  強引
  淫乱
  捕虜
  従順

gzinファイル名：通常はgzin9を指定してください。
- rai8,command

# chk_touch
女キャラのタッチシステムが有効かチェックします。
有効な場合は、chk_touch以降の処理を実行します。
無効な場合は、chk_touch_end以降の処理を実行します。
事前にz8_old_initを実行しておく必要があります。
- rai8,command,control

# chk_touch_end
chk_touch のジャンプ先
- rai8,command,control

# z8_old_touch
タッチオルドを開始します。
- rai8,command

# z8_old_file_init,男人物ID1:PersonId,女人物ID2:PersonId
800x600サイズのオルド画像表示のための初期化処理を行います。
z8_old_file_img、z8_old_file_flush、z8_old_file_cutinを使用する場合、この初期化処理が必要です。
- rai8,command

# z8_old_file_img,ファイル名:File,フェード:Choice|0:フェードあり|1:フェードなし
usr_dirフォルダにあるオルド画像ファイル（800x600）を表示します。

ファイル名: 800x600サイズの画像ファイル名を指定します。拡張子を含めた記述が必要です。jpg/pngのファイルを指定できます。

フェード
  1:フェードありで表示
  0:フェードなしで表示
- rai8,command

# z8_old_file_flush,ファイル名:File?
イベント画面をフラッシュします。
ファイル名が指定されている場合、フラッシュ後に画像が切り替わります。
ファイル名は省略可能です。
- rai8,command

# z8_old_se,SE_ID:Choice|0:停止|1:ぱんぱん等速|2:ぱんぱん倍速|3:射精音|4:挿入音|5:ごっくん1回|6:ごっくん3回|7:んぐっ

オルド特有のサウンド・エフェクトを再生します。
サウンドを停止する場合は、SE_IDに0を指定します。

SE_ID
  0:停止
  1:ぱんぱん等速(ループ)
  2:ぱんぱん倍速(ループ)
  3:射精音
  4:挿入音
  5:ごっくん1回
  6:ごっくん3回
  7:んぐっ
- rai8,command

# z8_biku320_init
340x340アニメの初期化を行います。
z8_old_320anime を使用する場合、この初期化処理が必要です。
- rai8,command

# z8_old_320anime,アニメモード:Choice|nk:中出し|so:潮吹き|ot:騎乗位
指定したモードでアニメーションを再生します。
事前に z8_old_init, z8_biku320_init を実行しておく必要があります。
z8_old_init で女キャラIDを設定しておく必要があります。

アニメモード
  nk:中出し 
  so:潮吹き  
  ot:騎乗位
- rai8,command

# z8_old_biku,揺れアニメID:Choice|1:アニメA|2:アニメB|3:アニメC
数秒間、画面が揺れます。
指定した揺れアニメIDによって異なる揺れ方をします。

揺れアニメID
  1:アニメA
  2:アニメB
  3:アニメC
- rai8,command

# z8_old_fin,着床:Choice|on:着床有効|off:着床無効
オルド終了処理を行います。
中出し回数が+1加算されます。
着床がonの場合、妊娠する可能性があります。offの場合は妊娠しません。

着床
  on:着床有効
  off:着床無効
- rai8,command

# z8_old_msg,人物ID:PersonId,台詞:Speech
オルドイベントでの台詞を表示します。
- rai8,command

# z8_flush,モード:Choice|1:女絶頂(未挿入)|2:女絶頂(挿入)|3:中出し|4:外出し|5:顔射|0:3-5のランダム
R8モードのオルド画面をフラッシュします。
フラッシュの後、モードに指定された差分画像が表示されます。

モード
  1:女絶頂(未挿入)
  2:女絶頂(挿入)
  3:中出し
  4:外出し
  5:顔射
  0:3-5のランダム
- rai8,command

# z8_old_nar,waitモード:Choice|0:ウェイトなし|1:ウェイトあり,ナレーション文字:Speech
オルドイベント(800x600)のナレーションを画面中央に表示します。
ウェイトありの場合、キー入力があるまで、次の処理に進みません。

waitモード
  0:ウェイトなし
  1:ウェイトあり
- rai8,command

# z8_old_img,画像種別:Choice|1:挿入なし|2:挿入あり|3:中出し|4:外出し|5:顔射,汗差分:Choice|on|off,汁差分:Choice|on|off,表情差分:Choice|1|2|3|4|5|6|7|8|9|10|11|12,フェード:Choice|on|off
R8モードのオルド画像(800x600)を表示します。
各差分の指定によって異なる画像になります。
事前にz8_old_initによる初期化が必要です。

画像種別
  1:挿入なし
  2:挿入あり
  3:中出し
  4:外出し
  5:顔射
汗差分   on/off
汁差分   on/off
表情差分 1-12
フェード:
  on:フェードあり
  off:フェードなし
- rai8,command

# z8_old_bgv_play,ボイスモード:Choice|ah|sd|nd|ir|aa|fr|sa|ks|kg|ia|gs|sa|yt,ループ再生:Choice|on|off,再生速度:Choice|x1:等速|x2:倍速
はぁはぁボイスをバックグランドで再生します。
事前にz8_old_init z8_biku320_initによる初期化が必要です。
停止するにはz8_old_bgv_stopを実行します。

ボイスモード ah/sd/nd/ir/aa/fr/sa/ks/kg/ia/gs/sa/yt
ループ再生   on/off
再生速度
  x1:標準速度再生
  x2:倍速再生
- rai8,command

# z8_old_bgv_stop
z8_old_bgv_play, z8_old_dvc_play による再生を停止します。
- rai8,command

# z8_old_load,p1:PersonId,p2:Any,p3:Choice|B|G|N
- rai8,command

# z8_gcntup,人物ID:PersonId
- rai8,command

# chk_r7mode,p1:PersonId
- rai8,command,control

# chk_r7mode_end
- rai8,command,control

# r7_setval,男人物ID:PersonId,女人物ID:PersonId,状況:Choice|和姦|強引|媚薬,p4:Any
- rai8,command

# r7_osload,p1:PersonId,p2:Any,p3:Any
- rai8,command

# set_namida,p1:Choice|on|off
- rai8,command

# dark_reback
- rai7,rai8,command

# set_ret,p1:Any
- rai8,command

# set_hoflg,p1:PersonId,p2:Any,p3:Any
- rai8,command

# aki_aje_union
- rai8,command
 
# set_pback,覇王ID:RulerId,人物ID:PersonId
- rai8,command
 
# rar_kisaku1
- rai8,command
# rar_kisaku2
- rai8,command

# sp_cmd05,p1:Any
- rai8,command,deprecated

# sp_cmd06,p1:Any
- rai8,command,deprecated

# sp_cmd[0-4]+
- rai8,command,regex,deprecated


