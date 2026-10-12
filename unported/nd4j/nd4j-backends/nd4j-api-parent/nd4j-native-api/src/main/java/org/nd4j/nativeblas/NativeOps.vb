Imports org.bytedeco.javacpp
Imports Cast = org.bytedeco.javacpp.annotation.Cast

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

Namespace org.nd4j.nativeblas


	Public Interface NativeOps
		''' <summary>
		''' This method allows you to specify minimal number of elements per thread/block during op call
		''' PLEASE NOTE: Changing this value might and will affect performance.
		''' </summary>
		''' <param name="value"> </param>
		WriteOnly Property ElementThreshold As Integer

		''' <summary>
		''' This method allows you to specify minimal number of TADs per thread/block during op call
		''' PLEASE NOTE: Changing this value might and will affect performance.
		''' </summary>
		''' <param name="value"> </param>
		WriteOnly Property TADThreshold As Integer

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParams"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execIndexReduceScalar(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dXShapeInfo, Pointer extraParams, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, @Cast("Nd4jLong *") LongPointer dZShapeInfo);
		Sub execIndexReduceScalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dXShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dZShapeInfo As LongPointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParams"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfoBuffer"> </param>
		''' <param name="dimension"> </param>
		''' <param name="dimensionLength"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execIndexReduce(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dXShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfoBuffer, @Cast("Nd4jLong *") LongPointer dResultShapeInfoBuffer, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape);
		Sub execIndexReduce(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dXShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dResultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="y"> </param>
		''' <param name="yShapeInfo"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
		''' <param name="dimension"> </param>
		''' <param name="dimensionLength"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execBroadcast(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape);
		Sub execBroadcast(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execBroadcastBool(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape);
		Sub execBroadcastBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)


		''' <param name="opNum"> </param>
		''' <param name="dx"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="y"> </param>
		''' <param name="yShapeInfo"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
		''' <param name="extraParams"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execPairwiseTransform(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams);
		Sub execPairwiseTransform(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execPairwiseTransformBool(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams);
		Sub execPairwiseTransformBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParams"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceFloat(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo);
		Sub execReduceFloat(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceSame(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo);
		Sub execReduceSame(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceBool(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo);
		Sub execReduceBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceLong(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo);
		Sub execReduceLong(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParams"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceFloat2(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape);
		Sub execReduceFloat2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceSame2(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape);
		Sub execReduceSame2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceBool2(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape);
		Sub execReduceBool2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduceLong2(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape);
		Sub execReduceLong2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParamsVals"> </param>
		''' <param name="y"> </param>
		''' <param name="yShapeInfo"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduce3(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParamsVals, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo);
		Sub execReduce3(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParamsVals"> </param>
		''' <param name="y"> </param>
		''' <param name="yShapeInfo"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduce3Scalar(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParamsVals, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, @Cast("Nd4jLong *") LongPointer dzShapeInfo);
		Sub execReduce3Scalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParamsVals"> </param>
		''' <param name="y"> </param>
		''' <param name="yShapeInfo"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfoBuffer"> </param>
		''' <param name="dimension"> </param>
		''' <param name="dimensionLength"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduce3Tad(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParamsVals, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfoBuffer, @Cast("Nd4jLong *") LongPointer dresultShapeInfoBuffer, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape, @Cast("Nd4jLong *") LongPointer tadOnlyShapeInfo, @Cast("Nd4jLong *") LongPointer tadOffsets, @Cast("Nd4jLong *") LongPointer yTadOnlyShapeInfo, @Cast("Nd4jLong *") LongPointer yTadOffsets);
		Sub execReduce3Tad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dresultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal tadOnlyShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal yTadOnlyShapeInfo As LongPointer, ByVal yTadOffsets As LongPointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execReduce3All(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParamsVals, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeInfo, @Cast("Nd4jLong *") LongPointer dyShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfoBuffer, @Cast("Nd4jLong *") LongPointer dresultShapeInfoBuffer, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape, @Cast("Nd4jLong *") LongPointer xTadShape, @Cast("Nd4jLong *") LongPointer xOffsets, @Cast("Nd4jLong *") LongPointer yTadShape, @Cast("Nd4jLong *") LongPointer yOffsets);
		Sub execReduce3All(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dresultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal xTadShape As LongPointer, ByVal xOffsets As LongPointer, ByVal yTadShape As LongPointer, ByVal yOffsets As LongPointer)


		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
		''' <param name="scalar"> </param>
		''' <param name="extraParams"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execScalar(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, OpaqueDataBuffer scalar, @Cast("Nd4jLong *") LongPointer scalarShapeInfo, @Cast("Nd4jLong *") LongPointer dscalarShapeInfo, Pointer extraParams);
		Sub execScalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal scalar As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execScalarBool(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, OpaqueDataBuffer scalar, @Cast("Nd4jLong *") LongPointer scalarShapeInfo, @Cast("Nd4jLong *") LongPointer dscalarShapeInfo, Pointer extraParams);
		Sub execScalarBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal scalar As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParams"> </param>
		''' <param name="biasCorrected"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execSummaryStatsScalar(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, @Cast("Nd4jLong *") LongPointer dzShapeInfo, boolean biasCorrected);
		Sub execSummaryStatsScalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal biasCorrected As Boolean)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParams"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
		''' <param name="biasCorrected"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execSummaryStats(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, boolean biasCorrected);
		Sub execSummaryStats(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal biasCorrected As Boolean)

		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="extraParams"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfoBuffer"> </param>
		''' <param name="dimension"> </param>
		''' <param name="dimensionLength"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execSummaryStatsTad(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer extraParams, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfoBuffer, @Cast("Nd4jLong *") LongPointer dresultShapeInfoBuffer, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape, boolean biasCorrected, @Cast("Nd4jLong *") LongPointer tadShapeInfo, @Cast("Nd4jLong *") LongPointer tadOffsets);
		Sub execSummaryStatsTad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dresultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal biasCorrected As Boolean, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer)


		''' <param name="extraPointers"> </param>
		''' <param name="opNum"> </param>
		''' <param name="dx"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="result"> </param>
		''' <param name="resultShapeInfo"> </param>
		''' <param name="extraParams"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execTransformFloat(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams);
		Sub execTransformFloat(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execTransformSame(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams);
		Sub execTransformSame(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execTransformStrict(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams);
		Sub execTransformStrict(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execTransformBool(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams);
		Sub execTransformBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execTransformAny(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer result, @Cast("Nd4jLong *") LongPointer resultShapeInfo, @Cast("Nd4jLong *") LongPointer dresultShapeInfo, Pointer extraParams);
		Sub execTransformAny(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)

		''' <summary>
		''' ScalarOp along dimension
		''' </summary>
		''' <param name="extraPointers">   pointers to tadShapes and tadoffsets </param>
		''' <param name="opNum"> </param>
		''' <param name="x"> </param>
		''' <param name="xShapeInfo"> </param>
		''' <param name="z"> </param>
		''' <param name="zShapeInfo"> </param>
		''' <param name="scalars"> </param>
		''' <param name="extraParams"> </param>
		''' <param name="dimension"> </param>
		''' <param name="dimensionLength"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execScalarTad(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, @Cast("Nd4jLong *") LongPointer dzShapeInfo, OpaqueDataBuffer scalars, @Cast("Nd4jLong *") LongPointer scalarShapeInfo, @Cast("Nd4jLong *") LongPointer dscalarShapeInfo, Pointer extraParams, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape, @Cast("Nd4jLong *") LongPointer tadShapeInfo, @Cast("Nd4jLong *") LongPointer tadOffsets, @Cast("Nd4jLong *") LongPointer tadShapeInfoZ, @Cast("Nd4jLong *") LongPointer tadOffsetsZ);
		Sub execScalarTad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal scalars As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal tadShapeInfoZ As LongPointer, ByVal tadOffsetsZ As LongPointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execScalarBoolTad(PointerPointer extraPointers, int opNum, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, @Cast("Nd4jLong *") LongPointer dzShapeInfo, OpaqueDataBuffer scalars, @Cast("Nd4jLong *") LongPointer scalarShapeInfo, @Cast("Nd4jLong *") LongPointer dscalarShapeInfo, Pointer extraParams, OpaqueDataBuffer hDimension, @Cast("Nd4jLong *") LongPointer hDimensionShape, @Cast("Nd4jLong *") LongPointer dDimensionShape, @Cast("Nd4jLong *") LongPointer tadShapeInfo, @Cast("Nd4jLong *") LongPointer tadOffsets, @Cast("Nd4jLong *") LongPointer tadShapeInfoZ, @Cast("Nd4jLong *") LongPointer tadOffsetsZ);
		Sub execScalarBoolTad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal scalars As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal tadShapeInfoZ As LongPointer, ByVal tadOffsetsZ As LongPointer)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void specialConcat(PointerPointer extraPointers, int dimension, int numArrays, PointerPointer data, PointerPointer inputShapeInfo, Pointer results, @Cast("Nd4jLong *") LongPointer resultShapeInfo, PointerPointer tadPointers, PointerPointer tadOffsets);
		Sub specialConcat(ByVal extraPointers As PointerPointer, ByVal dimension As Integer, ByVal numArrays As Integer, ByVal data As PointerPointer, ByVal inputShapeInfo As PointerPointer, ByVal results As Pointer, ByVal resultShapeInfo As LongPointer, ByVal tadPointers As PointerPointer, ByVal tadOffsets As PointerPointer)


		''' <summary>
		''' Gets the maximum number of open mp threads
		''' 
		''' @return
		''' </summary>
		Function ompGetMaxThreads() As Integer

		''' <summary>
		''' Gets the number of open mp threads
		''' 
		''' @return
		''' </summary>
		Function ompGetNumThreads() As Integer

		''' <summary>
		''' Sets the number of openmp threads
		''' </summary>
		''' <param name="threads"> </param>
		WriteOnly Property OmpNumThreads As Integer

		''' <summary>
		''' Sets the minimal number of openmp threads for variative methods
		''' </summary>
		''' <param name="threads"> </param>
		WriteOnly Property OmpMinThreads As Integer

		''' <summary>
		''' NEVER EVER USE THIS METHOD OUTSIDE OF  CUDA
		''' </summary>
		Sub initializeDevicesAndFunctions()

		Sub initializeFunctions(ByVal functions As PointerPointer)

		Function mallocHost(ByVal memorySize As Long, ByVal flags As Integer) As Pointer

		Function mallocDevice(ByVal memorySize As Long, ByVal ptrToDeviceId As Integer, ByVal flags As Integer) As Pointer

		Function freeHost(ByVal pointer As Pointer) As Integer

		Function freeDevice(ByVal pointer As Pointer, ByVal deviceId As Integer) As Integer

		Function createContext() As Pointer

		Function createStream() As Pointer

		Function createEvent() As Pointer

		Function registerEvent(ByVal [event] As Pointer, ByVal stream As Pointer) As Integer

		Function destroyEvent(ByVal [event] As Pointer) As Integer

		Function setDevice(ByVal ptrToDeviceId As Integer) As Integer

		ReadOnly Property Device As Integer

		Function streamSynchronize(ByVal stream As Pointer) As Integer

		Function eventSynchronize(ByVal [event] As Pointer) As Integer

		Function getDeviceFreeMemory(ByVal ptrToDeviceId As Integer) As Long

		ReadOnly Property DeviceFreeMemoryDefault As Long

		Function getDeviceTotalMemory(ByVal ptrToDeviceId As Integer) As Long

		Function getDeviceMajor(ByVal ptrToDeviceId As Integer) As Integer

		Function getDeviceMinor(ByVal ptrToDeviceId As Integer) As Integer

		Function getDeviceName(ByVal ptrToDeviceId As Integer) As String

		Function memcpySync(ByVal dst As Pointer, ByVal src As Pointer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer

		Function memcpyAsync(ByVal dst As Pointer, ByVal src As Pointer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer

		Function memcpyConstantAsync(ByVal dst As Long, ByVal src As Pointer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer

		Function memsetSync(ByVal dst As Pointer, ByVal value As Integer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer

		Function memsetAsync(ByVal dst As Pointer, ByVal value As Integer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer

		ReadOnly Property ConstantSpace As Pointer

		ReadOnly Property AvailableDevices As Integer

		Sub enableDebugMode(ByVal reallyEnable As Boolean)

		Sub enableVerboseMode(ByVal reallyEnable As Boolean)

		WriteOnly Property GridLimit As Integer

		Function tadOnlyShapeInfo(ByVal shapeInfo As LongPointer, ByVal dimension As IntPointer, ByVal dimensionLength As Integer) As OpaqueTadPack

		Function getPrimaryShapeInfo(ByVal pack As OpaqueTadPack) As LongPointer
		Function getPrimaryOffsets(ByVal pack As OpaqueTadPack) As LongPointer
		Function getSpecialShapeInfo(ByVal pack As OpaqueTadPack) As LongPointer
		Function getSpecialOffsets(ByVal pack As OpaqueTadPack) As LongPointer
		Function getNumberOfTads(ByVal pack As OpaqueTadPack) As Long
		Function getShapeInfoLength(ByVal pack As OpaqueTadPack) As Integer

		Sub deleteTadPack(ByVal pointer As OpaqueTadPack)

		'/////////////

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void pullRows(PointerPointer extraPointers, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, @Cast("Nd4jLong *") LongPointer dzShapeInfo, long n, @Cast("Nd4jLong *") LongPointer indexes, @Cast("Nd4jLong *") LongPointer tadShapeInfo, @Cast("Nd4jLong *") LongPointer tadOffsets, @Cast("Nd4jLong *") LongPointer zTadShapeInfo, @Cast("Nd4jLong *") LongPointer zTadOffsets);
		Sub pullRows(ByVal extraPointers As PointerPointer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal n As Long, ByVal indexes As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal zTadShapeInfo As LongPointer, ByVal zTadOffsets As LongPointer)


		'/////////////////////

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void average(PointerPointer extraPointers, PointerPointer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, PointerPointer dx, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, Pointer dz, @Cast("Nd4jLong *") LongPointer dzShapeInfo, int n, long length, boolean propagate);
		Sub average(ByVal extraPointers As PointerPointer, ByVal x As PointerPointer, ByVal xShapeInfo As LongPointer, ByVal dx As PointerPointer, ByVal dxShapeInfo As LongPointer, ByVal z As Pointer, ByVal zShapeInfo As LongPointer, ByVal dz As Pointer, ByVal dzShapeInfo As LongPointer, ByVal n As Integer, ByVal length As Long, ByVal propagate As Boolean)

		'/////////////////////

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void accumulate(PointerPointer extraPointers, PointerPointer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, PointerPointer dx, @Cast("Nd4jLong *") LongPointer dxShapeInfo, Pointer z, @Cast("Nd4jLong *") LongPointer zShapeInfo, Pointer dz, @Cast("Nd4jLong *") LongPointer dzShapeInfo, int n, long length);
		Sub accumulate(ByVal extraPointers As PointerPointer, ByVal x As PointerPointer, ByVal xShapeInfo As LongPointer, ByVal dx As PointerPointer, ByVal dxShapeInfo As LongPointer, ByVal z As Pointer, ByVal zShapeInfo As LongPointer, ByVal dz As Pointer, ByVal dzShapeInfo As LongPointer, ByVal n As Integer, ByVal length As Long)

		'/////////////////////

		Sub enableP2P(ByVal reallyEnable As Boolean)

		Sub checkP2P()

		ReadOnly Property P2PAvailable As Boolean

		'

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void shuffle(PointerPointer extraPointers, PointerPointer x, @Cast("Nd4jLong *") PointerPointer xShapeInfo, PointerPointer dx, @Cast("Nd4jLong *") PointerPointer dxShapeInfo, PointerPointer z, @Cast("Nd4jLong *") PointerPointer zShapeInfo, PointerPointer dz, @Cast("Nd4jLong *") PointerPointer dzShapeInfo, int N, IntPointer shuffleMap, PointerPointer tadShapeInfo, PointerPointer tadOffsets);
		Sub shuffle(ByVal extraPointers As PointerPointer, ByVal x As PointerPointer, ByVal xShapeInfo As PointerPointer, ByVal dx As PointerPointer, ByVal dxShapeInfo As PointerPointer, ByVal z As PointerPointer, ByVal zShapeInfo As PointerPointer, ByVal dz As PointerPointer, ByVal dzShapeInfo As PointerPointer, ByVal N As Integer, ByVal shuffleMap As IntPointer, ByVal tadShapeInfo As PointerPointer, ByVal tadOffsets As PointerPointer)


		' opType conversion

		Sub convertTypes(ByVal extras As PointerPointer, ByVal srcType As Integer, ByVal x As Pointer, ByVal N As Long, ByVal dstType As Integer, ByVal z As Pointer)

		ReadOnly Property ExperimentalEnabled As Boolean

		' GridOps

	'
	'    // MetaOps
	'    void execMetaPredicateShape(PointerPointer extras,
	'                                                int opTypeA, int opNumA,
	'                                                int opTypeB, int opNumB,
	'                                                long N,
	'                                                Pointer x, @Cast("Nd4jLong *") LongPointer xShape,
	'                                                Pointer dx, @Cast("Nd4jLong *") LongPointer dxShape,
	'                                                Pointer y, @Cast("Nd4jLong *") LongPointer yShape,
	'                                                Pointer dy, @Cast("Nd4jLong *") LongPointer dyShape,
	'                                                Pointer z, @Cast("Nd4jLong *") LongPointer zShape,
	'                                                Pointer dz, @Cast("Nd4jLong *") LongPointer dzShape,
	'                                                Pointer extraA, Pointer extraB, double scalarA,
	'                                                double scalarB);
	'
	'
		'///////////////////////

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execAggregate(PointerPointer extras, int opNum, PointerPointer arguments, int numArguments, @Cast("Nd4jLong **") PointerPointer shapes, int numShapes, IntPointer indexArguments, int numIndexArguments, @Cast("int **") PointerPointer intArrays, int numIntArrays, Pointer realArguments, int numRealArguments, @Cast("nd4j::DataType") int dataType);
		Sub execAggregate(ByVal extras As PointerPointer, ByVal opNum As Integer, ByVal arguments As PointerPointer, ByVal numArguments As Integer, ByVal shapes As PointerPointer, ByVal numShapes As Integer, ByVal indexArguments As IntPointer, ByVal numIndexArguments As Integer, ByVal intArrays As PointerPointer, ByVal numIntArrays As Integer, ByVal realArguments As Pointer, ByVal numRealArguments As Integer, ByVal dataType As Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execAggregateBatch(PointerPointer extras, int numAggregates, int opNum, int maxArgs, int maxShapes, int maxIntArrays, int maxIntArraySize, int maxIdx, int maxReals, Pointer ptrToArguments, @Cast("nd4j::DataType") int dataType);
		Sub execAggregateBatch(ByVal extras As PointerPointer, ByVal numAggregates As Integer, ByVal opNum As Integer, ByVal maxArgs As Integer, ByVal maxShapes As Integer, ByVal maxIntArrays As Integer, ByVal maxIntArraySize As Integer, ByVal maxIdx As Integer, ByVal maxReals As Integer, ByVal ptrToArguments As Pointer, ByVal dataType As Integer)


		'////////////
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execRandom(PointerPointer extraPointers, int opNum, Pointer state, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeBuffer, @Cast("Nd4jLong *") LongPointer dzShapeBuffer, Pointer extraArguments);
		Sub execRandom(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal state As Pointer, ByVal z As OpaqueDataBuffer, ByVal zShapeBuffer As LongPointer, ByVal dzShapeBuffer As LongPointer, ByVal extraArguments As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execRandom3(PointerPointer extraPointers, int opNum, Pointer state, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeBuffer, @Cast("Nd4jLong *") LongPointer dxShapeBuffer, OpaqueDataBuffer y, @Cast("Nd4jLong *") LongPointer yShapeBuffer, @Cast("Nd4jLong *") LongPointer dyShapeBuffer, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeBuffer, @Cast("Nd4jLong *") LongPointer dzShapeBuffer, Pointer extraArguments);
		Sub execRandom3(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal state As Pointer, ByVal x As OpaqueDataBuffer, ByVal xShapeBuffer As LongPointer, ByVal dxShapeBuffer As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeBuffer As LongPointer, ByVal dyShapeBuffer As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeBuffer As LongPointer, ByVal dzShapeBuffer As LongPointer, ByVal extraArguments As Pointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void execRandom2(PointerPointer extraPointers, int opNum, Pointer state, OpaqueDataBuffer x, @Cast("Nd4jLong *") LongPointer xShapeBuffer, @Cast("Nd4jLong *") LongPointer dxShapeBuffer, OpaqueDataBuffer z, @Cast("Nd4jLong *") LongPointer zShapeBuffer, @Cast("Nd4jLong *") LongPointer dzShapeBuffer, Pointer extraArguments);
		Sub execRandom2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal state As Pointer, ByVal x As OpaqueDataBuffer, ByVal xShapeBuffer As LongPointer, ByVal dxShapeBuffer As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeBuffer As LongPointer, ByVal dzShapeBuffer As LongPointer, ByVal extraArguments As Pointer)

		'//////////////////


		Function initRandom(ByVal extraPointers As PointerPointer, ByVal seed As Long, ByVal numberOfElements As Long, ByVal pointerToBuffer As Pointer) As Pointer

		Sub refreshBuffer(ByVal extraPointers As PointerPointer, ByVal seed As Long, ByVal pointer As Pointer)

		Sub reSeedBuffer(ByVal extraPointers As PointerPointer, ByVal seed As Long, ByVal pointer As Pointer)

		Sub destroyRandom(ByVal pointer As Pointer)


		''' <summary>
		''' Create a numpy array from an nd4j
		''' array
		''' </summary>
		''' <param name="data">        a pointer to the data </param>
		''' <param name="shapeBuffer"> the shapebuffer for the nd4j array </param>
		''' <param name="wordSize">    the word size (4 for float, 8 for doubles) </param>
		''' <returns> a pointer to a numpy array </returns>
		Function numpyFromNd4j(ByVal data As Pointer, ByVal shapeBuffer As Pointer, ByVal wordSize As Long) As Pointer


		''' <summary>
		''' Get the element size for a numpy array
		''' </summary>
		''' <param name="npyArray"> the numpy array's address
		'''                 to get the length for
		''' @return </param>
		Function elementSizeForNpyArrayHeader(ByVal npyArray As Pointer) As Integer


		''' <param name="npyArrayStruct">
		''' @return </param>
		Function dataPointForNumpyStruct(ByVal npyArrayStruct As Pointer) As Pointer


		''' <summary>
		''' Creates a numpy header for nd4j
		''' </summary>
		''' <param name="data">        the data to use </param>
		''' <param name="shapeBuffer"> the shape buffer for the array </param>
		''' <param name="wordSize">    the word size
		''' @return </param>
		Function numpyHeaderForNd4j(ByVal data As Pointer, ByVal shapeBuffer As Pointer, ByVal wordSize As Long, ByVal length As LongPointer) As Pointer

		''' <summary>
		''' Load numpy from a header
		''' based on the cnpy parse from header method.
		''' </summary>
		''' <param name="data"> the header data to parse </param>
		''' <returns> a pointer to a numpy cnpy:NpyArray struct </returns>
		Function loadNpyFromHeader(ByVal data As Pointer) As Pointer

		''' <param name="npyArray">
		''' @return </param>
		Function dataPointForNumpyHeader(ByVal npyArray As Pointer) As Pointer

		''' <summary>
		''' Get the shape buffer from a
		''' numpy array.
		''' **Warning** this allocates memory
		''' </summary>
		''' <param name="npyArray">
		''' @return </param>
		Function shapeBufferForNumpyHeader(ByVal npyArray As Pointer) As Pointer

		''' <summary>
		''' Used in <seealso cref="org.nd4j.linalg.factory.NDArrayFactory.createFromNpyPointer(Pointer)"/>
		''' to allow reuse of an in memory numpy buffer.
		''' This is heavily used for python interop
		''' </summary>
		''' <param name="npyArray"> the pointer to the numpy array to use </param>
		''' <returns> the pointer for the numpy array </returns>
		Function dataPointForNumpy(ByVal npyArray As Pointer) As Pointer

		''' <summary>
		''' Get a shape buffer for a numpy array.
		''' Used in conjunction with <seealso cref="org.nd4j.linalg.factory.NDArrayFactory.createFromNpyPointer(Pointer)"/>
		''' </summary>
		''' <param name="npyArray"> the numpy array to get the shape buffer for </param>
		''' <returns> a pointer representing the shape buffer for numpy </returns>
		Function shapeBufferForNumpy(ByVal npyArray As Pointer) As Pointer

		''' <summary>
		''' Thie method releases numpy pointer
		''' <para>
		''' PLEASE NOTE: This method should be ONLY used if pointer/numpy array was originated from file
		''' 
		''' </para>
		''' </summary>
		''' <param name="npyArray"> </param>
		Sub releaseNumpy(ByVal npyArray As Pointer)


		''' <summary>
		''' Create a numpy array pointer
		''' from a file
		''' </summary>
		''' <param name="path"> the path to the file
		''' @return </param>
		Function numpyFromFile(ByVal path As BytePointer) As Pointer


		''' <summary>
		''' Return the length of a shape buffer
		''' based on the pointer
		''' </summary>
		''' <param name="buffer"> the buffer pointer to check
		''' @return </param>
		Function lengthForShapeBufferPointer(ByVal buffer As Pointer) As Integer

		''' <summary>
		''' Calculate the element size
		''' for a numpy array
		''' </summary>
		''' <param name="npyArray"> the numpy array to get the
		'''                 element size for </param>
		''' <returns> the element size for a given array </returns>
		Function elementSizeForNpyArray(ByVal npyArray As Pointer) As Integer


		''' <summary>
		''' The pointer to get the address for
		''' </summary>
		''' <param name="address"> the address to get the pointer </param>
		''' <returns> the pointer for the given address </returns>
		Function pointerForAddress(ByVal address As Long) As Pointer


		'//// NPZ ///////
		Function mapFromNpzFile(ByVal path As BytePointer) As Pointer

		Function getNumNpyArraysInMap(ByVal map As Pointer) As Integer



		Function getNpyArrayNameFromMap(ByVal map As Pointer, ByVal index As Integer, ByVal buffer As BytePointer) As String

		Function getNpyArrayFromMap(ByVal map As Pointer, ByVal index As Integer) As Pointer

		Function getNpyArrayData(ByVal npArray As Pointer) As Pointer

		Function getNpyArrayShape(ByVal npArray As Pointer) As LongPointer

		Function getNpyArrayRank(ByVal npArray As Pointer) As Integer

		Function getNpyArrayOrder(ByVal npArray As Pointer) As Char

		Function getNpyArrayElemSize(ByVal npArray As Pointer) As Integer
		'/////


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void tear(PointerPointer extras, OpaqueDataBuffer tensor, @Cast("Nd4jLong *") LongPointer xShapeInfo, @Cast("Nd4jLong *") LongPointer dxShapeInfo, PointerPointer targets, @Cast("Nd4jLong *") LongPointer zShapeInfo, @Cast("Nd4jLong *") LongPointer tadShapeInfo, @Cast("Nd4jLong *") LongPointer tadOffsets);
		Sub tear(ByVal extras As PointerPointer, ByVal tensor As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal targets As PointerPointer, ByVal zShapeInfo As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void sort(PointerPointer extraPointers, Pointer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, Pointer dx, @Cast("Nd4jLong *") LongPointer dxShapeInfo, boolean descending);
		Sub sort(ByVal extraPointers As PointerPointer, ByVal x As Pointer, ByVal xShapeInfo As LongPointer, ByVal dx As Pointer, ByVal dxShapeInfo As LongPointer, ByVal descending As Boolean)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void sortTad(PointerPointer extraPointers, Pointer x, @Cast("Nd4jLong *") LongPointer xShapeInfo, Pointer dx, @Cast("Nd4jLong *") LongPointer dxShapeInfo, IntPointer dimension, int dimensionLength, @Cast("Nd4jLong *") LongPointer tadShapeInfo, @Cast("Nd4jLong *") LongPointer tadOffsets, boolean descending);
		Sub sortTad(ByVal extraPointers As PointerPointer, ByVal x As Pointer, ByVal xShapeInfo As LongPointer, ByVal dx As Pointer, ByVal dxShapeInfo As LongPointer, ByVal dimension As IntPointer, ByVal dimensionLength As Integer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal descending As Boolean)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void sortCooIndices(PointerPointer extraPointers, @Cast("Nd4jLong *") LongPointer indices, Pointer x, long length, @Cast("Nd4jLong *") LongPointer shapeInfo);
		Sub sortCooIndices(ByVal extraPointers As PointerPointer, ByVal indices As LongPointer, ByVal x As Pointer, ByVal length As Long, ByVal shapeInfo As LongPointer)


		''' 
		''' <param name="extraPointers">     not used </param>
		''' <param name="indices">           DataBuffer containing COO indices for a sparse matrix that is to be raveled/flattened </param>
		''' <param name="flatIndices">       DataBuffer where the raveled/flattened indices are to be written to </param>
		''' <param name="length">            number of non-zero entries (length of flatIndices) </param>
		''' <param name="shapeInfo">   DataBuffer with ShapeInfo for the full matrix to be flattened </param>
		''' <param name="mode">              clipMode determines the strategy to use if some of the the passed COO indices does
		'''                          not fit into the shape determined by fullShapeBuffer
		'''                              0   throw an exception (default)
		'''                              1   wrap around shape
		'''                              2   clip to shape </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void ravelMultiIndex(PointerPointer extraPointers, @Cast("Nd4jLong *") LongPointer indices, @Cast("Nd4jLong *") LongPointer flatIndices, long length, @Cast("Nd4jLong *") LongPointer shapeInfo, int mode);
		Sub ravelMultiIndex(ByVal extraPointers As PointerPointer, ByVal indices As LongPointer, ByVal flatIndices As LongPointer, ByVal length As Long, ByVal shapeInfo As LongPointer, ByVal mode As Integer)

		''' 
		''' <param name="extraPointers">     not used </param>
		''' <param name="indices">           DataBuffer where the unraveled COO indices are to be written </param>
		''' <param name="flatIndices">       DataBuffer containing the raveled/flattened indices to be unravel </param>
		''' <param name="length">            number of non-zero entries (length of flatIndices) </param>
		''' <param name="shapeInfo">   DataBuffer with ShapeInfo for the full matrix to be unraveled </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void unravelIndex(PointerPointer extraPointers, @Cast("Nd4jLong *") LongPointer indices, @Cast("Nd4jLong *") LongPointer flatIndices, long length, @Cast("Nd4jLong *") LongPointer shapeInfo);
		Sub unravelIndex(ByVal extraPointers As PointerPointer, ByVal indices As LongPointer, ByVal flatIndices As LongPointer, ByVal length As Long, ByVal shapeInfo As LongPointer)


		Function mmapFile(ByVal extraPointers As PointerPointer, ByVal fileName As String, ByVal length As Long) As LongPointer

		Sub munmapFile(ByVal extraPointers As PointerPointer, ByVal ptrMap As LongPointer, ByVal length As Long)

		Function executeFlatGraph(ByVal extraPointers As PointerPointer, ByVal flatBufferPointer As Pointer) As OpaqueResultWrapper

		Function getResultWrapperSize(ByVal ptr As OpaqueResultWrapper) As Long
		Function getResultWrapperPointer(ByVal ptr As OpaqueResultWrapper) As Pointer

		ReadOnly Property AllCustomOps As String

		ReadOnly Property AllOperations As String

		Function execCustomOp2(ByVal extraPointers As PointerPointer, ByVal opHashCode As Long, ByVal context As Pointer) As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: int execCustomOp(PointerPointer extraPointers, long opHashCode, PointerPointer inputBuffers, PointerPointer inputShapes, int numInput, PointerPointer outputBuffers, PointerPointer outputShapes, int numOutputs, DoublePointer tArgs, int numTArgs, @Cast("Nd4jLong *") LongPointer iArgs, int numIArgs, @Cast("bool *") BooleanPointer bArgs, int numBArgs, boolean isInplace);
		Function execCustomOp(ByVal extraPointers As PointerPointer, ByVal opHashCode As Long, ByVal inputBuffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal numInput As Integer, ByVal outputBuffers As PointerPointer, ByVal outputShapes As PointerPointer, ByVal numOutputs As Integer, ByVal tArgs As DoublePointer, ByVal numTArgs As Integer, ByVal iArgs As LongPointer, ByVal numIArgs As Integer, ByVal bArgs As BooleanPointer, ByVal numBArgs As Integer, ByVal isInplace As Boolean) As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: OpaqueShapeList calculateOutputShapes(PointerPointer extraPointers, long hash, PointerPointer inputShapes, int numInputShapes, DoublePointer tArgs, int numTArgs, @Cast("Nd4jLong *") LongPointer iArgs, int numIArgs);
		Function calculateOutputShapes(ByVal extraPointers As PointerPointer, ByVal hash As Long, ByVal inputShapes As PointerPointer, ByVal numInputShapes As Integer, ByVal tArgs As DoublePointer, ByVal numTArgs As Integer, ByVal iArgs As LongPointer, ByVal numIArgs As Integer) As OpaqueShapeList

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: OpaqueShapeList calculateOutputShapes2(PointerPointer extraPointers, long hash, PointerPointer inputBunffers, PointerPointer inputShapes, int numInputShapes, DoublePointer tArgs, int numTArgs, @Cast("Nd4jLong *") LongPointer iArgs, int numIArgs, @Cast("bool *") BooleanPointer bArgs, int numBArgs, @Cast("int *") IntPointer dArgs, int numDArgs);
		Function calculateOutputShapes2(ByVal extraPointers As PointerPointer, ByVal hash As Long, ByVal inputBunffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal numInputShapes As Integer, ByVal tArgs As DoublePointer, ByVal numTArgs As Integer, ByVal iArgs As LongPointer, ByVal numIArgs As Integer, ByVal bArgs As BooleanPointer, ByVal numBArgs As Integer, ByVal dArgs As IntPointer, ByVal numDArgs As Integer) As OpaqueShapeList

		Function getShapeListSize(ByVal list As OpaqueShapeList) As Long
		Function getShape(ByVal list As OpaqueShapeList, ByVal i As Long) As LongPointer

		Function registerGraph(ByVal extraPointers As PointerPointer, ByVal graphId As Long, ByVal flatBufferPointer As Pointer) As Integer

		Function executeStoredGraph(ByVal extraPointers As PointerPointer, ByVal graphId As Long, ByVal inputBuffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal inputIndices As IntPointer, ByVal numInputs As Integer) As OpaqueVariablesSet

		Function getVariablesSetSize(ByVal set As OpaqueVariablesSet) As Long
		Function getVariablesSetStatus(ByVal set As OpaqueVariablesSet) As Integer
		Function getVariable(ByVal set As OpaqueVariablesSet, ByVal i As Long) As OpaqueVariable
		Function getVariableId(ByVal variable As OpaqueVariable) As Integer
		Function getVariableIndex(ByVal variable As OpaqueVariable) As Integer
		Function getVariableName(ByVal variable As OpaqueVariable) As String
		Function getVariableShape(ByVal variable As OpaqueVariable) As LongPointer
		Function getVariableBuffer(ByVal variable As OpaqueVariable) As Pointer

		Sub deleteResultWrapper(ByVal ptr As Pointer)

		Sub deleteShapeList(ByVal ptr As Pointer)

		Function unregisterGraph(ByVal extraPointers As PointerPointer, ByVal graphId As Long) As Integer

		Sub deleteIntArray(ByVal pointer As Pointer)

		Sub deleteLongArray(ByVal pointer As Pointer)

		Sub deletePointerArray(ByVal pointer As Pointer)

		Sub deleteNPArrayStruct(ByVal pointer As Pointer)

		Sub deleteNPArrayMap(ByVal pointer As Pointer)

		Sub deleteVariablesSet(ByVal pointer As OpaqueVariablesSet)

		' GraphState creation
		Function getGraphState(ByVal id As Long) As Pointer

		Sub deleteGraphState(ByVal state As Pointer)

		Function estimateThreshold(ByVal extraPointers As PointerPointer, ByVal x As Pointer, ByVal xShapeInfo As LongPointer, ByVal N As Integer, ByVal threshold As Single) As Integer

		' this method executes op that requires scope to be present: if/while/cond/whatever
		Function execCustomOpWithScope(ByVal extraPointers As PointerPointer, ByVal state As Pointer, ByVal opHash As Long, ByVal scopes() As Long, ByVal numScopes As Integer, ByVal inputBuffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal numInputs As Integer, ByVal outputBuffers As PointerPointer, ByVal outputShapes As PointerPointer, ByVal numOutputs As Integer) As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void scatterUpdate(PointerPointer extraPointers, int opCode, int numOfUpdates, Pointer hX, @Cast("Nd4jLong *") LongPointer hXShapeInfo, @Cast("Nd4jLong *") LongPointer hxOffsets, Pointer dX, @Cast("Nd4jLong *") LongPointer dXShapeInfo, @Cast("Nd4jLong *") LongPointer dxOffsets, Pointer hY, @Cast("Nd4jLong *") LongPointer hYShapeInfo, @Cast("Nd4jLong *") LongPointer hyOffsets, Pointer dY, @Cast("Nd4jLong *") LongPointer dYShapeInfo, @Cast("Nd4jLong *") LongPointer dyOffsets, Pointer hIndices, @Cast("Nd4jLong *") LongPointer hIndicesShapeInfo, Pointer dIndices, @Cast("Nd4jLong *") LongPointer dIndicesShapeInfo);
		Sub scatterUpdate(ByVal extraPointers As PointerPointer, ByVal opCode As Integer, ByVal numOfUpdates As Integer, ByVal hX As Pointer, ByVal hXShapeInfo As LongPointer, ByVal hxOffsets As LongPointer, ByVal dX As Pointer, ByVal dXShapeInfo As LongPointer, ByVal dxOffsets As LongPointer, ByVal hY As Pointer, ByVal hYShapeInfo As LongPointer, ByVal hyOffsets As LongPointer, ByVal dY As Pointer, ByVal dYShapeInfo As LongPointer, ByVal dyOffsets As LongPointer, ByVal hIndices As Pointer, ByVal hIndicesShapeInfo As LongPointer, ByVal dIndices As Pointer, ByVal dIndicesShapeInfo As LongPointer)

		'void fillUtf8String(PointerPointer extraPointers, String[] string, int numStrings, Pointer buffer);
		Function createUtf8String(ByVal extraPointers As PointerPointer, ByVal [string] As String, ByVal length As Integer) As Pointer
		Function getUtf8StringLength(ByVal extraPointers As PointerPointer, ByVal ptr As Pointer) As Long
		Function getUtf8StringBuffer(ByVal extraPointers As PointerPointer, ByVal ptr As Pointer) As BytePointer
		Sub deleteUtf8String(ByVal extraPointers As PointerPointer, ByVal ptr As Pointer)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void inspectArray(PointerPointer extraPointers, Pointer buffer, @Cast("Nd4jLong *") LongPointer shapeInfo, Pointer specialBuffer, @Cast("Nd4jLong *") LongPointer specialShapeInfo, @Cast("nd4j::DebugInfo *") Pointer debugInfo);
		Sub inspectArray(ByVal extraPointers As PointerPointer, ByVal buffer As Pointer, ByVal shapeInfo As LongPointer, ByVal specialBuffer As Pointer, ByVal specialShapeInfo As LongPointer, ByVal debugInfo As Pointer)

		''' <summary>
		''' this method tries to read numBytes bytes from buffer to provoke crash in certain scenarios
		''' </summary>
		Sub tryPointer(ByVal extras As Pointer, ByVal buffer As Pointer, ByVal numBytesToRead As Integer)


		''' <summary>
		''' This method returns data type from npy header
		''' 
		''' PLEASE NOTE: dont use output directly, use DataType.fromInt(output) instead </summary>
		''' <param name="numpyHeader">
		''' @return </param>
		Function dataTypeFromNpyHeader(ByVal numpyHeader As Pointer) As Integer

		Function shapeBuffer(ByVal rank As Integer, ByVal shape As LongPointer, ByVal strides As LongPointer, ByVal dtype As Integer, ByVal order As Char, ByVal ews As Long, ByVal empty As Boolean) As OpaqueConstantShapeBuffer

		Function shapeBufferEx(ByVal rank As Integer, ByVal shape As LongPointer, ByVal strides As LongPointer, ByVal dtype As Integer, ByVal order As Char, ByVal ews As Long, ByVal extras As Long) As OpaqueConstantShapeBuffer

		Function constantBufferDouble(ByVal dtype As Integer, ByVal data As DoublePointer, ByVal length As Integer) As OpaqueConstantDataBuffer

		Function constantBufferLong(ByVal dtype As Integer, ByVal data As LongPointer, ByVal length As Integer) As OpaqueConstantDataBuffer

		Function getConstantDataBufferPrimary(ByVal dbf As OpaqueConstantDataBuffer) As Pointer
		Function getConstantDataBufferSpecial(ByVal dbf As OpaqueConstantDataBuffer) As Pointer
		Function getConstantDataBufferLength(ByVal dbf As OpaqueConstantDataBuffer) As Long

		Function getConstantShapeBufferPrimary(ByVal dbf As OpaqueConstantShapeBuffer) As Pointer
		Function getConstantShapeBufferSpecial(ByVal dbf As OpaqueConstantShapeBuffer) As Pointer

		Sub deleteConstantShapeBuffer(ByVal state As OpaqueConstantShapeBuffer)
		Sub deleteConstantDataBuffer(ByVal state As OpaqueConstantDataBuffer)

		Function createGraphContext(ByVal nodeId As Integer) As OpaqueContext
		Function getGraphContextRandomGenerator(ByVal ptr As OpaqueContext) As OpaqueRandomGenerator
		Sub markGraphContextInplace(ByVal ptr As OpaqueContext, ByVal reallyInplace As Boolean)
		Sub setGraphContextCudaContext(ByVal ptr As OpaqueContext, ByVal stream As Pointer, ByVal reductionPointer As Pointer, ByVal allocationPointer As Pointer)
		Sub setGraphContextInputArray(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal buffer As Pointer, ByVal shapeInfo As Pointer, ByVal specialBuffer As Pointer, ByVal specialShapeInfo As Pointer)
		Sub setGraphContextOutputArray(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal buffer As Pointer, ByVal shapeInfo As Pointer, ByVal specialBuffer As Pointer, ByVal specialShapeInfo As Pointer)
		Sub setGraphContextInputBuffer(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal databuffer As OpaqueDataBuffer, ByVal shapeInfo As Pointer, ByVal specialShapeInfo As Pointer)
		Sub setGraphContextOutputBuffer(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal databuffer As OpaqueDataBuffer, ByVal shapeInfo As Pointer, ByVal specialShapeInfo As Pointer)
		Sub setGraphContextTArguments(ByVal ptr As OpaqueContext, ByVal arguments As DoublePointer, ByVal numberOfArguments As Integer)
		Sub setGraphContextIArguments(ByVal ptr As OpaqueContext, ByVal arguments As LongPointer, ByVal numberOfArguments As Integer)
		Sub setGraphContextDArguments(ByVal ptr As OpaqueContext, ByVal arguments As IntPointer, ByVal numberOfArguments As Integer)
		Sub setGraphContextBArguments(ByVal ptr As OpaqueContext, ByVal arguments As BooleanPointer, ByVal numberOfArguments As Integer)
		Sub ctxAllowHelpers(ByVal ptr As OpaqueContext, ByVal reallyAllow As Boolean)
		Sub ctxSetExecutionMode(ByVal ptr As OpaqueContext, ByVal execMode As Integer)
		Sub ctxShapeFunctionOverride(ByVal ptr As OpaqueContext, ByVal reallyOverride As Boolean)
		Sub ctxPurge(ByVal ptr As OpaqueContext)
		Sub deleteGraphContext(ByVal ptr As OpaqueContext)

		Function createRandomGenerator(ByVal rootSeed As Long, ByVal nodeSeed As Long) As OpaqueRandomGenerator
		Function getRandomGeneratorRootState(ByVal ptr As OpaqueRandomGenerator) As Long
		Function getRandomGeneratorNodeState(ByVal ptr As OpaqueRandomGenerator) As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void setRandomGeneratorStates(OpaqueRandomGenerator ptr, @Cast("Nd4jLong") long rootSeed, @Cast("Nd4jLong") long nodeSeed);
		Sub setRandomGeneratorStates(ByVal ptr As OpaqueRandomGenerator, ByVal rootSeed As Long, ByVal nodeSeed As Long)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: float getRandomGeneratorRelativeFloat(OpaqueRandomGenerator ptr, @Cast("Nd4jLong") long index);
		Function getRandomGeneratorRelativeFloat(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Single
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: double getRandomGeneratorRelativeDouble(OpaqueRandomGenerator ptr, @Cast("Nd4jLong") long index);
		Function getRandomGeneratorRelativeDouble(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Double
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: int getRandomGeneratorRelativeInt(OpaqueRandomGenerator ptr, @Cast("Nd4jLong") long index);
		Function getRandomGeneratorRelativeInt(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: long getRandomGeneratorRelativeLong(OpaqueRandomGenerator ptr, @Cast("Nd4jLong") long index);
		Function getRandomGeneratorRelativeLong(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Long
		Sub deleteRandomGenerator(ByVal ptr As OpaqueRandomGenerator)



		Function getCachedMemory(ByVal deviceId As Integer) As Long

		Function defaultLaunchContext() As OpaqueLaunchContext

		Function lcScalarPointer(ByVal lc As OpaqueLaunchContext) As Pointer
		Function lcReductionPointer(ByVal lc As OpaqueLaunchContext) As Pointer
		Function lcAllocationPointer(ByVal lc As OpaqueLaunchContext) As Pointer
		Function lcExecutionStream(ByVal lc As OpaqueLaunchContext) As Pointer
		Function lcCopyStream(ByVal lc As OpaqueLaunchContext) As Pointer
		Function lcBlasHandle(ByVal lc As OpaqueLaunchContext) As Pointer
		Function lcSolverHandle(ByVal lc As OpaqueLaunchContext) As Pointer

		Function lastErrorCode() As Integer
		Function lastErrorMessage() As String

		Function isBlasVersionMatches(ByVal major As Integer, ByVal minor As Integer, ByVal build As Integer) As Boolean

		Function binaryLevel() As Integer
		Function optimalLevel() As Integer

		ReadOnly Property MinimalRequirementsMet As Boolean
		ReadOnly Property OptimalRequirementsMet As Boolean


		Function allocateDataBuffer(ByVal elements As Long, ByVal dataType As Integer, ByVal allocateBoth As Boolean) As OpaqueDataBuffer
		Function dbAllocateDataBuffer(ByVal elements As Long, ByVal dataType As Integer, ByVal allocateBoth As Boolean) As OpaqueDataBuffer
		Function dbCreateExternalDataBuffer(ByVal elements As Long, ByVal dataType As Integer, ByVal primary As Pointer, ByVal special As Pointer) As OpaqueDataBuffer
		Function dbCreateView(ByVal dataBuffer As OpaqueDataBuffer, ByVal length As Long, ByVal offset As Long) As OpaqueDataBuffer
		Function dbPrimaryBuffer(ByVal dataBuffer As OpaqueDataBuffer) As Pointer
		Function dbSpecialBuffer(ByVal dataBuffer As OpaqueDataBuffer) As Pointer
		Sub dbExpandBuffer(ByVal dataBuffer As OpaqueDataBuffer, ByVal elements As Long)
		Sub dbAllocatePrimaryBuffer(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbAllocateSpecialBuffer(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbSetPrimaryBuffer(ByVal dataBuffer As OpaqueDataBuffer, ByVal primaryBuffer As Pointer, ByVal numBytes As Long)
		Sub dbSetSpecialBuffer(ByVal dataBuffer As OpaqueDataBuffer, ByVal specialBuffer As Pointer, ByVal numBytes As Long)
		Sub dbSyncToSpecial(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbSyncToPrimary(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbTickHostRead(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbTickHostWrite(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbTickDeviceRead(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbTickDeviceWrite(ByVal dataBuffer As OpaqueDataBuffer)
		Sub deleteDataBuffer(ByVal dataBuffer As OpaqueDataBuffer)
		Sub dbClose(ByVal dataBuffer As OpaqueDataBuffer)
		Function dbLocality(ByVal dataBuffer As OpaqueDataBuffer) As Integer
		Function dbDeviceId(ByVal dataBuffer As OpaqueDataBuffer) As Integer
		Sub dbSetDeviceId(ByVal dataBuffer As OpaqueDataBuffer, ByVal deviceId As Integer)
		Sub dbExpand(ByVal dataBuffer As OpaqueDataBuffer, ByVal newLength As Long)

		''' <summary>
		''' Gets the build information of the backend
		''' 
		''' @return
		''' </summary>
		Function buildInfo() As String
	End Interface

End Namespace