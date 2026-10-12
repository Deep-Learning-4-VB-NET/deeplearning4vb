Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports BTools = org.nd4j.common.tools.BTools
Imports InfoLine = org.nd4j.common.tools.InfoLine
Imports InfoValues = org.nd4j.common.tools.InfoValues
Imports SIS = org.nd4j.common.tools.SIS

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.linalg.util


	Public Class DataSetUtils
		'
		Private sis As SIS
		'
		Public Sub New(ByVal sis As SIS, ByVal superiorModuleCode As String)
			'
			Me.sis = sis
			'
			initValues(superiorModuleCode)
		End Sub
		'
		Private ReadOnly baseModuleCode As String = "DL4JT"
		Private moduleCode As String = ""
		'

		Private Sub initValues(ByVal superiorModuleCode As String)
			'
			moduleCode = superiorModuleCode & "." & baseModuleCode
			'
		End Sub

		''' <summary>
		''' <b>showDataSet</b><br>
		''' public void showDataSet( int mtLv, String itemCode, DataSet ds,<br>
		'''   int in_Digits, int ot_Digits, int r_End_I, int c_End_I )<br>
		''' Shows content of DataSet.<br> </summary>
		''' <param name="mtLv"> - method level </param>
		''' <param name="itemCode"> - item = DataSet </param>
		''' <param name="ds"> - DataSet </param>
		''' <param name="in_Digits"> - input digits </param>
		''' <param name="ot_Digits"> - output digits </param>
		''' <param name="r_End_I"> - rows end index </param>
		''' <param name="c_End_I"> - columns end index </param>

		Public Overridable Sub showDataSet(ByVal mtLv As Integer, ByVal itemCode As String, ByVal ds As DataSet, ByVal in_Digits As Integer, ByVal ot_Digits As Integer, ByVal r_End_I As Integer, ByVal c_End_I As Integer)
			'
			mtLv += 1
			'
			Dim oinfo As String = ""
			'
			Dim methodName As String = moduleCode & "." & "showDataSet"
			'
			If ds Is Nothing Then
				oinfo = ""
				oinfo &= BTools.getMtLvESS(mtLv)
				oinfo &= methodName & ": "
				oinfo &= """" & itemCode & """: "
				oinfo &= " == null !!!; "
				oinfo &= BTools.SLcDtTm
				sis.info(oinfo)
				Return
			End If
			'
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= methodName & ": "
			oinfo &= """" & itemCode & """: "
			oinfo &= "in_Digits: " & in_Digits & "; "
			oinfo &= "ot_Digits: " & ot_Digits & "; "
			sis.info(oinfo)
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= BTools.MtLvISS
			oinfo &= "r_End_I: " & r_End_I & "; "
			oinfo &= "c_End_I: " & c_End_I & "; "
			oinfo &= BTools.SLcDtTm
			sis.info(oinfo)
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= BTools.MtLvISS
			oinfo &= "ds: "
			oinfo &= ".numInputs: " & ds.numInputs() & "; "
			oinfo &= ".numOutcomes: " & ds.numOutcomes() & "; "
			oinfo &= ".numExamples: " & ds.numExamples() & "; "
			oinfo &= ".hasMaskArrays: " & BTools.getSBln(ds.hasMaskArrays()) & "; "
			sis.info(oinfo)
			'
			If in_Digits < 0 Then
				in_Digits = 0
			End If
			If ot_Digits < 0 Then
				ot_Digits = 0
			End If
			'
			Dim in_INDA As INDArray ' I = Input
			Dim ot_INDA As INDArray ' O = Output
			'
			in_INDA = ds.Features
			ot_INDA = ds.Labels
			'
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= BTools.MtLvISS
			oinfo &= "in_INDA: "
			oinfo &= ".rows: " & in_INDA.rows() & "; "
			oinfo &= ".columns: " & in_INDA.columns() & "; "
			oinfo &= ".rank: " & in_INDA.rank() & "; "
			oinfo &= ".shape: " & BTools.getSIntA(ArrayUtil.toInts(in_INDA.shape())) & "; "
			oinfo &= ".length: " & in_INDA.length() & "; "
			oinfo &= ".size( 0 ): " & in_INDA.size(0) & "; "
			sis.info(oinfo)
			'
			If ot_INDA IsNot Nothing Then
				oinfo = ""
				oinfo &= BTools.getMtLvESS(mtLv)
				oinfo &= BTools.MtLvISS
				oinfo &= "ot_INDA: "
				oinfo &= ".rows: " & ot_INDA.rows() & "; "
				oinfo &= ".columns: " & ot_INDA.columns() & "; "
				oinfo &= ".rank: " & ot_INDA.rank() & "; "
				oinfo &= ".shape: " & BTools.getSIntA(ArrayUtil.toInts(ot_INDA.shape())) & "; "
				oinfo &= ".length: " & ot_INDA.length() & "; "
				oinfo &= ".size( 0 ): " & ot_INDA.size(0) & "; "
				sis.info(oinfo)
			Else
				oinfo = ""
				oinfo &= BTools.getMtLvESS(mtLv)
				oinfo &= BTools.MtLvISS
				oinfo &= "ot_INDA == null ! "
				sis.info(oinfo)
			End If
			'
			If in_INDA.rows() <> ot_INDA.rows() Then
				oinfo = "==="
				oinfo &= methodName & ": "
				oinfo &= "in_INDA.rows() != ot_INDA.rows() !!! ; "
				oinfo &= BTools.SLcDtTm
				sis.info(oinfo)
				'
				Return
			End If
			'
			Dim wasShownTitle As Boolean = False
			'
			Dim il As InfoLine
			Dim iv As InfoValues
			'
			Dim j_Dbl As Double = -1
			If in_INDA.rows() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim i_CharsCount As Integer = BTools.getIndexCharsCount(CInt(in_INDA.rows()) - 1)
			'
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= BTools.MtLvISS
			oinfo &= "Data: j: IN->I0; "
			sis.info(oinfo)
			'
			Dim i As Integer = 0
			Do While i < in_INDA.rows()
				'
				If i > r_End_I Then
					Exit Do
				End If
				'
				il = New InfoLine()
				'
				iv = New InfoValues("i", "")
				il.ivL.Add(iv)
				iv.vsL.Add(BTools.getSInt(i, i_CharsCount))
				'
				iv = New InfoValues("", "", "")
				il.ivL.Add(iv)
				iv.vsL.Add("")
				'
				Dim c_I As Integer = 0
				'
				For j As Integer = CInt(in_INDA.columns()) - 1 To 0 Step -1
					'
					If c_I > c_End_I Then
						Exit For
					End If
					'
					j_Dbl = in_INDA.getDouble(i, j)
					'
					iv = New InfoValues("In", "j", BTools.getSInt(j))
					il.ivL.Add(iv)
					iv.vsL.Add(BTools.getSDbl(j_Dbl, in_Digits, True, in_Digits + 4))
					'
					c_I += 1
				Next j
				'
				iv = New InfoValues("", "", "")
				il.ivL.Add(iv)
				iv.vsL.Add("")
				'
				c_I = 0
				'
				If ot_INDA IsNot Nothing Then
					If ot_INDA.columns() - 1 > Integer.MaxValue Then
						Throw New ND4JArraySizeException()
					End If
					For j As Integer = CInt(ot_INDA.columns()) - 1 To 0 Step -1
						'
						If c_I > c_End_I Then
							Exit For
						End If
						'
						j_Dbl = ot_INDA.getDouble(i, j)
						'
						iv = New InfoValues("Ot", "j", BTools.getSInt(j))
						il.ivL.Add(iv)
						iv.vsL.Add(BTools.getSDbl(j_Dbl, ot_Digits, True, ot_Digits + 4))
						'
						c_I += 1
					Next j
				End If
				'
				If Not wasShownTitle Then
					oinfo = il.getTitleLine(mtLv, 0)
					sis.info(oinfo)
					oinfo = il.getTitleLine(mtLv, 1)
					sis.info(oinfo)
					oinfo = il.getTitleLine(mtLv, 2)
					sis.info(oinfo)
	'			    oinfo = il.getTitleLine( mtLv, 3 ); sis.info( oinfo );
	'			    oinfo = il.getTitleLine( mtLv, 4 ); sis.info( oinfo );
					wasShownTitle = True
				End If
				oinfo = il.getValuesLine(mtLv)
				sis.info(oinfo)
				i += 1
			Loop
			'
		End Sub

		''' <summary>
		''' <b>showINDArray</b><br>
		''' public void showINDArray( int mtLv, String itemCode, INDArray INDA,<br>
		'''   int digits, int r_End_I, int c_End_I )<br>
		''' Shows content of INDArray.<br>
		''' Shows first rows and than columns.<br>
		''' 
		''' 
		''' </summary>
		''' <param name="mtLv"> - method level </param>
		''' <param name="itemCode"> - item code </param>
		''' <param name="INDA"> - INDArray </param>
		''' <param name="digits"> - values digits </param>
		''' <param name="r_End_I"> - rows end index </param>
		''' <param name="c_End_I"> - columns end index </param>
		Public Overridable Sub showINDArray(ByVal mtLv As Integer, ByVal itemCode As String, ByVal INDA As INDArray, ByVal digits As Integer, ByVal r_End_I As Integer, ByVal c_End_I As Integer)
			'
			showINDArray(mtLv, itemCode, INDA, digits, r_End_I, c_End_I, False)
		End Sub

		''' <summary>
		''' <b>showINDArray</b><br>
		''' public void showINDArray( int mtLv, String itemCode, INDArray INDA,<br>
		'''   int digits, int r_End_I, int c_End_I, boolean turned )<br>
		''' Shows content of INDArray.<br>
		''' If turned is false shows first rows and than columns.<br>
		''' If turned is true shows first columns and than rows.<br> </summary>
		''' <param name="mtLv"> - method level </param>
		''' <param name="itemCode"> - item code </param>
		''' <param name="INDA"> - INDArray </param>
		''' <param name="digits"> - values digits </param>
		''' <param name="r_End_I"> - rows end index </param>
		''' <param name="c_End_I"> - columns end index </param>
		''' <param name="turned"> - turned rows and columns  </param>
		Public Overridable Sub showINDArray(ByVal mtLv As Integer, ByVal itemCode As String, ByVal INDA As INDArray, ByVal digits As Integer, ByVal r_End_I As Integer, ByVal c_End_I As Integer, ByVal turned As Boolean)
			'
			mtLv += 1
			'
			Dim oinfo As String = ""
			'
			Dim methodName As String = moduleCode & "." & "showINDArray"
			'
			If INDA Is Nothing Then
				oinfo = ""
				oinfo &= BTools.getMtLvESS(mtLv)
				oinfo &= methodName & ": "
				oinfo &= """" & itemCode & """: "
				oinfo &= " == null !!!; "
				oinfo &= BTools.SLcDtTm
				sis.info(oinfo)
				Return
			End If
			'
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= methodName & ": "
			oinfo &= """" & itemCode & """: "
			oinfo &= "digits: " & digits & "; "
			oinfo &= "r_End_I: " & r_End_I & "; "
			oinfo &= "c_End_I: " & c_End_I & "; "
			oinfo &= "turned: " & turned & "; "
			oinfo &= BTools.SLcDtTm
			sis.info(oinfo)
			'
			If digits < 0 Then
				digits = 0
			End If
			'
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= BTools.MtLvISS
			oinfo &= "rows: " & INDA.rows() & "; "
			oinfo &= "columns: " & INDA.columns() & "; "
			oinfo &= "rank: " & INDA.rank() & "; "
			oinfo &= "shape: " & BTools.getSIntA(ArrayUtil.toInts(INDA.shape())) & "; "
			oinfo &= "length: " & INDA.length() & "; "
			oinfo &= "size( 0 ): " & INDA.size(0) & "; "
			sis.info(oinfo)
			'
			Dim wasShownTitle As Boolean = False
			'
			Dim il As InfoLine
			Dim iv As InfoValues
			'
			Dim j_Dbl As Double = -1
			If INDA.rows() - 1 > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim i_CharsCount As Integer = BTools.getIndexCharsCount(CInt(INDA.rows()) - 1)
			'
			If Not turned Then '= standard
				oinfo = ""
				oinfo &= BTools.getMtLvESS(mtLv)
				oinfo &= BTools.MtLvISS
				oinfo &= "Data: j: IN->I0; "
				sis.info(oinfo)
				'
				Dim i As Integer = 0
				Do While i < INDA.rows()
					'
					If i > r_End_I Then
						Exit Do
					End If
					'
					il = New InfoLine()
					'
					iv = New InfoValues("i", "")
					il.ivL.Add(iv)
					iv.vsL.Add(BTools.getSInt(i, i_CharsCount))
					'
					Dim c_I As Integer = 0
					If INDA.columns() - 1 > Integer.MaxValue Then
						Throw New ND4JArraySizeException()
					End If
					For j As Integer = CInt(INDA.columns()) - 1 To 0 Step -1
						'
						If c_I > c_End_I Then
							Exit For
						End If
						'
						j_Dbl = INDA.getDouble(i, j)
						'
						iv = New InfoValues("j", "", BTools.getSInt(j))
						il.ivL.Add(iv)
						iv.vsL.Add(BTools.getSDbl(j_Dbl, digits, True, digits + 4))
						'
						c_I += 1
					Next j
					'
					If Not wasShownTitle Then
						oinfo = il.getTitleLine(mtLv, 0)
						sis.info(oinfo)
						oinfo = il.getTitleLine(mtLv, 1)
						sis.info(oinfo)
						oinfo = il.getTitleLine(mtLv, 2)
						sis.info(oinfo)
	'				    oinfo = il.getTitleLine( mtLv, 3 ); sis.info( oinfo );
	'				    oinfo = il.getTitleLine( mtLv, 4 ); sis.info( oinfo );
						wasShownTitle = True
					End If
					oinfo = il.getValuesLine(mtLv)
					sis.info(oinfo)
					i += 1
				Loop
			Else ' = turned
				oinfo = ""
				oinfo &= BTools.getMtLvESS(mtLv)
				oinfo &= BTools.MtLvISS
				oinfo &= "Data: "
				sis.info(oinfo)
				'
				Dim i As Integer = 0
				Do While i < INDA.columns()
					'
					If i > c_End_I Then
						Exit Do
					End If
					'
					il = New InfoLine()
					'
					iv = New InfoValues("i", "")
					il.ivL.Add(iv)
					iv.vsL.Add(BTools.getSInt(i, i_CharsCount))
					'
					Dim r_I As Integer = 0
					'
					Dim j As Integer = 0
					Do While j < INDA.rows()
						'
						If r_I > r_End_I Then
							Exit Do
						End If
						'
						j_Dbl = INDA.getDouble(j, i)
						'
						iv = New InfoValues("j", "", BTools.getSInt(j))
						il.ivL.Add(iv)
						iv.vsL.Add(BTools.getSDbl(j_Dbl, digits, True, digits + 4))
						'
						r_I += 1
						j += 1
					Loop
					'
					If Not wasShownTitle Then
						oinfo = il.getTitleLine(mtLv, 0)
						sis.info(oinfo)
						oinfo = il.getTitleLine(mtLv, 1)
						sis.info(oinfo)
						oinfo = il.getTitleLine(mtLv, 2)
						sis.info(oinfo)
	'				    oinfo = il.getTitleLine( mtLv, 3 ); sis.info( oinfo );
	'				    oinfo = il.getTitleLine( mtLv, 4 ); sis.info( oinfo );
						wasShownTitle = True
					End If
					oinfo = il.getValuesLine(mtLv)
					sis.info(oinfo)
					i += 1
				Loop
			End If
			'
		End Sub




	End Class

End Namespace