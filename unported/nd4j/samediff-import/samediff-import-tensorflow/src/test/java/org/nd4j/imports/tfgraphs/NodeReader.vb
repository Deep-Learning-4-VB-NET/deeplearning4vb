Imports System
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource

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

Namespace org.nd4j.imports.tfgraphs


	Public Class NodeReader
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray readArray(@NonNull String graph, @NonNull String variable) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readArray(ByVal graph As String, ByVal variable As String) As INDArray
			Dim shapeFile As File = Nothing
			Try
				shapeFile = (New ClassPathResource("tf_graphs/examples/" & graph & "/" & variable & ".prediction_inbw.shape")).File
			Catch e As Exception
				Try
					shapeFile = (New ClassPathResource("tf_graphs/examples/" & graph & "/" & variable & ".shape")).File
				Catch e1 As Exception
					Throw New Exception(e)
				End Try
			End Try

			Dim valuesFile As File = Nothing
			Try
				valuesFile = (New ClassPathResource("tf_graphs/examples/" & graph & "/" & variable & ".prediction_inbw.csv")).File
			Catch e As Exception
				Try
					valuesFile = (New ClassPathResource("tf_graphs/examples/" & graph & "/" & variable &".csv")).File
				Catch e1 As Exception
					Throw New Exception(e)
				End Try
			End Try

			Dim shapeLines As val = Files.readAllLines(shapeFile.toPath())
			Dim valuesLines As val = Files.readAllLines(valuesFile.toPath())

			Dim shape As val = New Long(shapeLines.size() - 1){}
			Dim values As val = New Double(valuesLines.size() - 1){}
			Dim cnt As Integer = 0
			For Each v As val In shapeLines
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: shape[cnt++] = System.Convert.ToInt64(v);
				shape(cnt) = Convert.ToInt64(v)
					cnt += 1
			Next v

			cnt = 0
			For Each v As val In valuesLines
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: values[cnt++] = System.Convert.ToDouble(v);
				values(cnt) = Convert.ToDouble(v)
					cnt += 1
			Next v

			Return Nd4j.create(values, shape)
		End Function
	End Class

End Namespace