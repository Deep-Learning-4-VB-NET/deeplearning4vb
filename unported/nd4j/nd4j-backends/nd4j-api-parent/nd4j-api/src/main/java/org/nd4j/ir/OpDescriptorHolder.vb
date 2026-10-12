Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports IOUtils = org.apache.commons.io.IOUtils
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TextFormat = org.nd4j.shade.protobuf.TextFormat

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
Namespace org.nd4j.ir


	''' <summary>
	''' A utility class for accessing the nd4j op descriptors.
	''' May override default definition of <seealso cref="nd4jFileNameTextDefault"/>
	''' with the system property <seealso cref="nd4jFileSpecifierProperty"/>
	''' @author Adam Gibson
	''' </summary>
	Public Class OpDescriptorHolder

		Public Shared nd4jFileNameTextDefault As String = "/nd4j-op-def.pbtxt"
		Public Shared nd4jFileSpecifierProperty As String = "samediff.import.nd4jdescriptors"
		Public Shared INSTANCE As OpNamespace.OpDescriptorList
		Private Shared opDescriptorByName As IDictionary(Of String, OpNamespace.OpDescriptor)

		Shared Sub New()
			Try
				INSTANCE = nd4jOpList()
			Catch e As IOException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try

			opDescriptorByName = New LinkedHashMap(Of String, OpNamespace.OpDescriptor)()
			Dim i As Integer = 0
			Do While i < INSTANCE.OpListCount
				opDescriptorByName(INSTANCE.getOpList(i).Name) = INSTANCE.getOpList(i)
				i += 1
			Loop

		End Sub

		''' <summary>
		''' Return the <seealso cref="org.nd4j.ir.OpNamespace.OpDescriptor"/>
		''' for a given op name </summary>
		''' <param name="name"> the name of the op to get the descriptor for </param>
		''' <returns> the desired op descriptor or null if it does not exist </returns>
		Public Shared Function descriptorForOpName(ByVal name As String) As OpNamespace.OpDescriptor
			Return opDescriptorByName(name)
		End Function

		''' <summary>
		''' Returns an singleton of the <seealso cref="nd4jOpList()"/>
		''' result, useful for preventing repeated I/O.
		''' @return
		''' </summary>
		Public Shared Function opList() As OpNamespace.OpDescriptorList
			Return INSTANCE
		End Function

		''' <summary>
		''' Get the nd4j op list <seealso cref="OpNamespace.OpDescriptorList"/> for serialization.
		''' Useful for saving and loading <seealso cref="org.nd4j.linalg.api.ops.DynamicCustomOp"/> </summary>
		''' <returns> the static list of descriptors from the nd4j classpath. </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static OpNamespace.OpDescriptorList nd4jOpList() throws java.io.IOException
		Public Shared Function nd4jOpList() As OpNamespace.OpDescriptorList
			Dim fileName As val = System.getProperty(nd4jFileSpecifierProperty, nd4jFileNameTextDefault)
			Dim nd4jOpDescriptorResourceStream As val = (New ClassPathResource(fileName, ND4JClassLoading.Nd4jClassloader)).InputStream
			Dim resourceString As val = IOUtils.toString(nd4jOpDescriptorResourceStream, Charset.defaultCharset())
			Dim descriptorListBuilder As val = OpNamespace.OpDescriptorList.newBuilder()
			TextFormat.merge(resourceString,descriptorListBuilder)
			Dim ret As val = descriptorListBuilder.build()
			Dim mutableList As val = New List(Of )(ret.getOpListList())
'JAVA TO VB CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to VB Converter:
			Collections.sort(mutableList, System.Collections.IComparer.comparing(OpNamespace.OpDescriptor::getName))

			Dim newResultBuilder As val = OpNamespace.OpDescriptorList.newBuilder()
			newResultBuilder.addAllOpList(mutableList)
			Return newResultBuilder.build()
		End Function

	End Class

End Namespace