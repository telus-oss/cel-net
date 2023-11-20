mkdir binary
for %%f in (*.textproto) do (
    protoc --proto_path=..\..\..\proto --encode=google.api.expr.test.v1.SimpleTestFile ..\..\..\proto\test\v1\simple.proto ..\..\..\proto\test\v1\proto2\test_all_types.proto ..\..\..\proto\test\v1\proto3\test_all_types.proto < %%~nf.textproto > binary\%%~nf.binpb    
)

