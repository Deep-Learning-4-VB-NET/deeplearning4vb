Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Shape = org.nd4j.linalg.api.shape.Shape

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

Namespace org.nd4j.linalg.api.ndarray


	Public Class JvmShapeInfo
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final long[] javaShapeInformation;
		Protected Friend ReadOnly javaShapeInformation() As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final long[] shape;
		Protected Friend ReadOnly shape() As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final long[] stride;
		Protected Friend ReadOnly stride() As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final long length;
		Protected Friend ReadOnly length As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final long ews;
		Protected Friend ReadOnly ews As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final long extras;
		Protected Friend ReadOnly extras As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final char order;
		Protected Friend ReadOnly order As Char
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final int rank;
		Protected Friend ReadOnly rank As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public JvmShapeInfo(@NonNull long[] javaShapeInformation)
		Public Sub New(ByVal javaShapeInformation() As Long)
			Me.javaShapeInformation = javaShapeInformation
			Me.shape = Shape.shape(javaShapeInformation)
			Me.stride = Shape.stride(javaShapeInformation)
			Me.length = If(Shape.isEmpty(javaShapeInformation), 0, Shape.length(javaShapeInformation))
			Me.ews = Shape.elementWiseStride(javaShapeInformation)
			Me.extras = Shape.extras(javaShapeInformation)
			Me.order = Shape.order(javaShapeInformation)
			Me.rank = Shape.rank(javaShapeInformation)
		End Sub
	End Class

End Namespace