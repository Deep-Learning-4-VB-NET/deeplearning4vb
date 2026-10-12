Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.primitives
Imports ArgDescriptorProposal = org.nd4j.descriptor.proposal.ArgDescriptorProposal
Imports ArgDescriptorSource = org.nd4j.descriptor.proposal.ArgDescriptorSource
Imports JavaSourceArgDescriptorSource = org.nd4j.descriptor.proposal.impl.JavaSourceArgDescriptorSource
Imports Libnd4jArgDescriptorSource = org.nd4j.descriptor.proposal.impl.Libnd4jArgDescriptorSource
Imports ArgDescriptorParserUtils = org.nd4j.descriptor.proposal.impl.ArgDescriptorParserUtils
Imports OpNamespace = org.nd4j.ir.OpNamespace
Imports TextFormat = org.nd4j.shade.protobuf.TextFormat

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.descriptor



	''' <summary>
	''' Parses the libnd4j code base based on a relative path
	''' default of ../deeplearning4j/libnd4j
	''' or a passed in file path.
	''' It generates a descriptor for each op.
	''' The file properties can be found at <seealso cref="OpDeclarationDescriptor"/>
	''' 
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class ParseOpFile


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void main(String...args) throws Exception
		Public Shared Sub Main(ParamArray ByVal args() As String)
			Dim libnd4jPath As String = If(args.Length > 0, args(0), Libnd4jArgDescriptorSource.DEFAULT_LIBND4J_DIRECTORY)
			Dim outputFilePath As String = If(args.Length > 1, args(1), ArgDescriptorParserUtils.DEFAULT_OUTPUT_FILE)

			Dim libnd4jRootDir As New File(libnd4jPath)
			Dim nd4jApiSourceDir As New StringBuilder()
			nd4jApiSourceDir.Append("nd4j")
			nd4jApiSourceDir.Append(File.separator)
			nd4jApiSourceDir.Append("nd4j-backends")
			nd4jApiSourceDir.Append(File.separator)
			nd4jApiSourceDir.Append("nd4j-api-parent")
			nd4jApiSourceDir.Append(File.separator)
			nd4jApiSourceDir.Append("nd4j-api")
			nd4jApiSourceDir.Append(File.separator)
			nd4jApiSourceDir.Append("src")
			nd4jApiSourceDir.Append(File.separator)
			nd4jApiSourceDir.Append("main")
			nd4jApiSourceDir.Append(File.separator)
			nd4jApiSourceDir.Append("java")
			Dim nd4jApiRootDir As New File(Path.GetDirectoryName(libnd4jPath),nd4jApiSourceDir.ToString())
			Console.WriteLine("Parsing  libnd4j code base at " & libnd4jRootDir.getAbsolutePath() & " and writing to " & outputFilePath)
			Dim libnd4jArgDescriptorSource As Libnd4jArgDescriptorSource = Libnd4jArgDescriptorSource.builder().libnd4jPath(libnd4jPath).weight(99999.0).build()



			Dim javaSourceArgDescriptorSource As JavaSourceArgDescriptorSource = JavaSourceArgDescriptorSource.builder().nd4jApiRootDir(nd4jApiRootDir).weight(1.0).build()

			Dim opTypes As IDictionary(Of String, OpNamespace.OpDescriptor.OpDeclarationType) = New Dictionary(Of String, OpNamespace.OpDescriptor.OpDeclarationType)()

			Dim proposals As IDictionary(Of String, IList(Of ArgDescriptorProposal)) = New Dictionary(Of String, IList(Of ArgDescriptorProposal))()
			For Each argDescriptorSource As ArgDescriptorSource In New ArgDescriptorSource() {libnd4jArgDescriptorSource, javaSourceArgDescriptorSource}
				Dim currProposals As IDictionary(Of String, IList(Of ArgDescriptorProposal)) = argDescriptorSource.getProposals()
				For Each entry As KeyValuePair(Of String, IList(Of ArgDescriptorProposal)) In currProposals.SetOfKeyValuePairs()
					Preconditions.checkState(Not entry.Key.isEmpty())
					Dim seenNames As ISet(Of String) = New HashSet(Of String)()
					If proposals.ContainsKey(entry.Key) Then
						Dim currProposalsList As IList(Of ArgDescriptorProposal) = proposals(entry.Key)
						CType(currProposalsList, List(Of ArgDescriptorProposal)).AddRange(entry.Value.Where(Function(proposal)
						Preconditions.checkState(Not proposal.getDescriptor().getName().isEmpty())
						Dim ret As Boolean = proposal.getDescriptor().getArgIndex() >= 0 AndAlso Not seenNames.Contains(proposal.getDescriptor().getName())
						seenNames.Add(proposal.getDescriptor().getName())
						Return ret
						End Function).ToList())

					Else
						Preconditions.checkState(Not entry.Key.isEmpty())
						proposals(entry.Key) = entry.Value
					End If
				Next entry
			Next argDescriptorSource

			javaSourceArgDescriptorSource.getOpTypes().forEach(Sub(k,v)
			opTypes(k) = OpNamespace.OpDescriptor.OpDeclarationType.valueOf(v.name())
			End Sub)

			libnd4jArgDescriptorSource.getOpTypes().forEach(Sub(k,v)
			opTypes(k) = OpNamespace.OpDescriptor.OpDeclarationType.valueOf(v.name())
			End Sub)

			opTypes.PutAll(javaSourceArgDescriptorSource.getOpTypes())
			opTypes.PutAll(libnd4jArgDescriptorSource.getOpTypes())

			Dim listBuilder As OpNamespace.OpDescriptorList.Builder = OpNamespace.OpDescriptorList.newBuilder()
			For Each proposal As KeyValuePair(Of String, IList(Of ArgDescriptorProposal)) In proposals.SetOfKeyValuePairs()
				Preconditions.checkState(Not proposal.Key.isEmpty())
'JAVA TO VB CONVERTER TODO TASK: Most Java stream collectors are not converted by Java to VB Converter:
				Dim collect As IDictionary(Of String, IList(Of ArgDescriptorProposal)) = proposal.Value.collect(Collectors.groupingBy(Function(input) input.getDescriptor().getName()))
				'merge boolean and int64
				collect.SetOfKeyValuePairs().forEach(Sub(entry)
				ArgDescriptorParserUtils.standardizeTypes(entry.getValue())
				End Sub)

				Dim rankedProposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), OpNamespace.ArgDescriptor) = ArgDescriptorParserUtils.standardizeNames(collect, proposal.Key)
				Dim opDescriptorBuilder As OpNamespace.OpDescriptor.Builder = OpNamespace.OpDescriptor.newBuilder().setOpDeclarationType(opTypes(proposal.Key)).setName(proposal.Key)
				rankedProposals.SetOfKeyValuePairs().Select(Function(input) input.getValue()).ForEach(Sub(argDescriptor)
				opDescriptorBuilder.addArgDescriptor(argDescriptor)
				End Sub)

				listBuilder.addOpList(opDescriptorBuilder.build())

			Next proposal

			Dim sortedListBuilder As OpNamespace.OpDescriptorList.Builder = OpNamespace.OpDescriptorList.newBuilder()
			Dim sortedDescriptors As IList(Of OpNamespace.OpDescriptor) = New List(Of OpNamespace.OpDescriptor)()
			Dim i As Integer = 0
			Do While i < listBuilder.OpListCount
				Dim opList As OpNamespace.OpDescriptor = listBuilder.getOpList(i)
				Dim sortedOpBuilder As OpNamespace.OpDescriptor.Builder = OpNamespace.OpDescriptor.newBuilder()
'JAVA TO VB CONVERTER TODO TASK: Most Java stream collectors are not converted by Java to VB Converter:
				Dim sortedByType As IDictionary(Of OpNamespace.ArgDescriptor.ArgType, IList(Of OpNamespace.ArgDescriptor)) = opList.getArgDescriptorList().collect(Collectors.groupingBy(Function(input) input.getArgType()))
				Dim namesEncountered As ISet(Of String) = New HashSet(Of String)()
				sortedByType.SetOfKeyValuePairs().forEach(Sub(entry)
				Collections.sort(entry.getValue(),System.Collections.IComparer.comparing(Function(inputArg) inputArg.getArgIndex()))
				Dim j As Integer = 0
				Do While j < entry.getValue().size()
					Dim currDescriptor As OpNamespace.ArgDescriptor = entry.getValue().get(j)
					Dim isArrayArg As Boolean = False
					Dim finalName As String = currDescriptor.Name
					If currDescriptor.Name.Contains("[") Then
						isArrayArg = True
						finalName = finalName.replaceAll("\[.*\]","").Replace("*","")
					End If
					If currDescriptor.ArgIndex <> j Then
						Throw New System.InvalidOperationException("Op name " & opList.Name & " has incontiguous indices for type " & entry.getKey() & " with descriptor being " & currDescriptor)
					End If
					Dim newDescriptor As OpNamespace.ArgDescriptor.Builder = OpNamespace.ArgDescriptor.newBuilder().setName(finalName).setArgIndex(currDescriptor.ArgIndex).setIsArray(isArrayArg).setArgType(currDescriptor.ArgType).setConvertBoolToInt(currDescriptor.ConvertBoolToInt)
					sortedOpBuilder.addArgDescriptor(newDescriptor.build())
					namesEncountered.Add(currDescriptor.Name)
					j += 1
				Loop
				End Sub)

				sortedOpBuilder.OpDeclarationType = opList.OpDeclarationType
				sortedOpBuilder.Name = opList.Name
				sortedDescriptors.Add(sortedOpBuilder.build())

				i += 1
			Loop


			'sort alphabetically
			sortedDescriptors.Sort(System.Collections.IComparer.comparing(Function(opDescriptor) opDescriptor.getName()))
			'add placeholder as an op to map
			sortedDescriptors.Add(OpNamespace.OpDescriptor.newBuilder().setName("placeholder").setOpDeclarationType(OpNamespace.OpDescriptor.OpDeclarationType.LOGIC_OP_IMPL).build())
			sortedDescriptors.ForEach(Sub(input)
			sortedListBuilder.addOpList(input)
			End Sub)


			Dim write As String = TextFormat.printToString(sortedListBuilder.build())
			FileUtils.writeStringToFile(New File(outputFilePath),write, Charset.defaultCharset())
		End Sub


	End Class

End Namespace